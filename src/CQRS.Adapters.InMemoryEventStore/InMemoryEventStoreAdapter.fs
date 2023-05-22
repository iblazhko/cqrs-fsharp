namespace CQRS.Adapters

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Threading.Tasks
open CQRS.Ports.EventStore

[<Sealed>]
type InMemoryEventStreamSession<'TEvent, 'TState>(eventStream: EventStream, eventMapper: IEventMapper<'TEvent>) =
    let mutable sessionNewEvents: EventWithMetadata list = []
    let mutable isLocked = false

    let assertSessionIsNotLocked () =
        if isLocked then
            raise (SessionIsLockedException eventStream.StreamId)

    let newVersion () =
        if sessionNewEvents.Length > 0 then
            eventStream.StreamVersion |> EventStreamVersion.increment
        else
            eventStream.StreamVersion

    let allEvents () =
        eventStream.Events |> Seq.append sessionNewEvents

    interface IEventStreamSession<'TEvent, 'TState> with
        member this.GetAllEvents() =
            task {
                assertSessionIsNotLocked ()

                return
                    { eventStream with
                        Events = allEvents ()
                        StreamVersion = newVersion () }
            }

        member this.GetNewEvents() =
            task {
                assertSessionIsNotLocked ()

                return
                    { eventStream with
                        Events = sessionNewEvents
                        StreamVersion = newVersion () }
            }

        member this.AppendEvents newEvents =
            task {
                assertSessionIsNotLocked ()

                sessionNewEvents <-
                    sessionNewEvents
                    |> List.append (
                        newEvents
                        |> Seq.filter (fun x -> not (isNull x))
                        |> Seq.map (fun x -> x |> eventMapper.FromDomainEvent)
                        |> Seq.map (fun x ->
                            { Event = x
                              EventType = x.GetType()
                              Metadata = None })
                        |> Seq.toList
                    )
            }

        member this.AppendEventsWithMetadata newEvents =
            task {
                assertSessionIsNotLocked ()

                sessionNewEvents <-
                    (sessionNewEvents
                     |> List.append (newEvents |> Seq.filter (fun x -> not (isNull x.Event)) |> Seq.toList))
            }

        member this.GetState projection =
            task {
                let initialState = projection.GetInitialState(eventStream.StreamId)

                let applyWithProjection state eventWithMetadata =
                    let domainEvent = eventWithMetadata |> eventMapper.ToDomainEvent
                    projection.Apply(state, domainEvent)

                return (allEvents ()) |> Seq.fold applyWithProjection initialState
            }

        member this.Lock() = isLocked <- true

        member this.Dispose() = ()
        member this.DisposeAsync() = ValueTask.CompletedTask

[<Sealed>]
type InMemoryEventStore(serializer: IEventSerializer, eventPublisher: IEventPublisher option) =
    let eventStreams = ConcurrentDictionary<EventStreamId, SerializedEventStream>()
    let eventTypes = ConcurrentDictionary<string, Type>()

    let getTypeByName name =
        eventTypes.GetOrAdd(
            name,
            (fun x ->
                let t = Type.GetType(x)

                if isNull t then
                    raise (UnknownEventTypeException x)

                t)
        )

    interface IEventStore with
        member this.Contains streamId =
            task { return eventStreams.ContainsKey(streamId) }

        member this.Delete(streamId) =
            task {
                let removed, _ = eventStreams.Remove(streamId)
                return removed
            }

        member this.Open<'TEvent, 'TState>(streamId, eventMapper) =
            task {
                let hasElement, element = eventStreams.TryGetValue(streamId)

                let serializedEventStream =
                    match hasElement with
                    | true -> element
                    | false ->
                        { StreamId = streamId
                          StreamVersion = EventStreamVersion.NewStream
                          Events = Seq.empty }

                let deserializedEvents =
                    serializedEventStream.Events
                    |> Seq.map (fun x ->
                        let eventType = getTypeByName x.EventTypeName
                        let e = serializer.Deserialize(x.Event, eventType)

                        { Event = e
                          EventType = eventType
                          Metadata = x.Metadata })

                let eventStream =
                    { StreamId = serializedEventStream.StreamId
                      StreamVersion = serializedEventStream.StreamVersion
                      Events = deserializedEvents }

                return new InMemoryEventStreamSession<'TEvent, 'TState>(eventStream, eventMapper)
            }

        member this.Save(session) =
            task {
                let! eventStream = session.GetNewEvents()

                let serializedEvents =
                    eventStream.Events
                    |> Seq.map (fun x ->
                        let se = serializer.Serialize(x.Event, x.EventType)

                        { Event = se
                          EventTypeName = x.EventType.FullName
                          Metadata = x.Metadata })
                    |> Seq.toList

                let serializedEventStream =
                    { SerializedEventStream.StreamId = eventStream.StreamId
                      StreamVersion = eventStream.StreamVersion
                      Events = serializedEvents }

                // publishing *could be* moved before serialization
                // however we want to be sure that we are ready to persist
                // before we start publishing
                do!
                    match eventPublisher with
                    | Some p -> p.Publish(eventStream.Events)
                    | None -> Task.CompletedTask

                eventStreams.AddOrUpdate(
                    eventStream.StreamId,
                    (fun _ -> serializedEventStream),
                    (fun _ -> (fun _ -> serializedEventStream))
                )
                |> ignore

                session.Lock()
            }

        member this.Dispose() = ()
        member this.DisposeAsync() = ValueTask.CompletedTask

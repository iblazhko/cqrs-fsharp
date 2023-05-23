namespace CQRS.Adapters

open System
open System.Collections.Generic
open System.Threading.Tasks
open CQRS.Ports.EventStore
open Marten

[<Sealed>]
type MartenDbEventStreamSession<'TEvent, 'TState>
    (eventStreamId: EventStreamId, documentStore: IDocumentStore, eventMapper: IEventMapper<'TEvent>) =

    let session = documentStore.LightweightSession()

    let mutable eventStream =
        { EventStream.StreamId = eventStreamId
          StreamVersion = EventStreamVersion.New
          Events = Seq.empty }

    let mutable sessionNewEvents: EventWithMetadata list = []
    let mutable isLocked = false

    let assertSessionIsNotLocked () =
        if isLocked then
            raise (SessionIsLockedException eventStreamId)

    let allEvents () =
        eventStream.Events |> Seq.append sessionNewEvents


    let mapMartenEvents (mtEvents: IReadOnlyList<Marten.Events.IEvent>) =
        mtEvents
        |> Seq.map (fun mtEvent ->
            { Event = mtEvent.Data
              EventType = mtEvent.GetType()
              Metadata = None })

    member val EventStreamId: EventStreamId = eventStreamId with get
    member val MartenSession: IDocumentSession = session with get

    member this.Open() =
        task {
            let! mtEvents = session.Events.FetchStreamAsync(eventStreamId)

            eventStream <-
                match mtEvents with
                | x when (isNull x) || x.Count = 0 -> eventStream
                | x ->
                    { eventStream with
                        StreamVersion = x[x.Count - 1].Version
                        Events = x |> mapMartenEvents }

            return ()
        }

    interface IEventStreamSession<'TEvent, 'TState> with
        member this.GetAllEvents() =
            task {
                assertSessionIsNotLocked ()

                return
                    { eventStream with
                        Events = allEvents () }
            }

        member this.GetNewEvents() =
            task {
                assertSessionIsNotLocked ()

                return
                    { eventStream with
                        Events = sessionNewEvents }
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

        member this.Dispose() =
            if not (isNull session) then
                session.Dispose()

        member this.DisposeAsync() =
            if not (isNull session) then
                session.DisposeAsync()
            else
                ValueTask.CompletedTask

[<Sealed>]
type MartenDbEventStore(documentStore: IDocumentStore, eventPublisher: IEventPublisher option) =
    let save (session: MartenDbEventStreamSession<'TEvent, 'TState>) =
        task {
            let sessionAsInterface = session :> IEventStreamSession<'TEvent, 'TState>
            let! eventStream = sessionAsInterface.GetNewEvents()

            do!
                match eventPublisher with
                | Some p -> p.Publish(eventStream.Events)
                | None -> Task.CompletedTask

            let mtEvents = eventStream.Events |> Seq.map (fun x -> x.Event)

            match eventStream.StreamVersion with
            | EventStreamVersion.New ->
                session.MartenSession.Events.StartStream(session.EventStreamId, mtEvents)
                |> ignore
            | _ -> session.MartenSession.Events.Append(session.EventStreamId, mtEvents) |> ignore

            do! session.MartenSession.SaveChangesAsync()

            sessionAsInterface.Lock()
        }

    interface IEventStore with
        member this.Contains _ =
            task {
                raise (NotImplementedException())
                return false
            }

        member this.Delete _ =
            task {
                raise (NotImplementedException())
                return false
            }

        member this.Open<'TEvent, 'TState>(streamId, eventMapper) =
            task {
                let eventStreamSession =
                    new MartenDbEventStreamSession<'TEvent, 'TState>(streamId, documentStore, eventMapper)

                do! eventStreamSession.Open()

                return eventStreamSession :> IEventStreamSession<'TEvent, 'TState>
            }

        member this.Save(session) =
            match session with
            | :? MartenDbEventStreamSession<'TEvent, 'TState> as mtSession -> save mtSession
            | _ -> failwith $"Wrong session type passed to Save"

        // MartenDb IDocumentStore instance lifecycle is managed by the application host
        // hence no disposing is necessary here
        member this.Dispose() = ()
        member this.DisposeAsync() = ValueTask.CompletedTask

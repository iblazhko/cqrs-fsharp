module CQRS.Application.Tests.EventStoreSessionTests

open CQRS.Adapters.EventStore
open CQRS.Application
open CQRS.Domain.Inventory
open CQRS.Domain.ValueTypes
open CQRS.EntityIds
open CQRS.Ports.EventStore
open FsUnit
open Xunit

let private getRandomStreamId () =
    EntityId.newId () |> EntityId.toString |> EventStreamId.create

let private eventMapper = InventoryEventStreamDtoMapper()

let private eventStore =
    new InMemoryEventStore(SystemTextJsonEventSerializer(), None) :> IEventStore

[<Fact>]
let ``EventStoreSession GetAllEvents return a new EventStream if the underlying stream is empty`` () =
    task {
        let streamId = getRandomStreamId ()
        use! eventStreamSession = eventStore.Open<InventoryEvent, InventoryState>(streamId, eventMapper)
        let! stream = eventStreamSession.GetAllEvents()

        stream.StreamId |> should equal streamId
        stream.StreamVersion |> should equal 0L
        stream.Events |> should equalSeq Seq.empty<EventWithMetadata>
    }

[<Fact>]
let ``EventStoreSession AppendEvents appends events with no metadata`` () =
    task {
        let inventoryId = EntityId.newId () |> InventoryId.create

        let inventoryName =
            "New Inventory"
            |> MediumString.create "InventoryName"
            |> Result.defaultWith (fun _ -> failwith "Failed to create inventory name")
            |> InventoryName.create

        let streamId = inventoryId |> InventoryEventStreamId.fromInventoryId

        use! eventStreamSession = eventStore.Open<InventoryEvent, InventoryState>(streamId, eventMapper)

        let domainEvents =
            [ InventoryEvent.InventoryCreated
                  { InventoryCreated.InventoryId = inventoryId
                    Name = inventoryName
                    IsActive = true } ]

        let genericEvents = domainEvents |> List.map (fun e -> e :> obj)

        do! eventStreamSession.AppendEvents(genericEvents)

    // No explicit assert here - in case of an error AppendEvents will raise an exception
    }

[<Fact>]
let ``EventStoreSession AppendEventsWithMetadata appends events with provided metadata`` () =
    task {
        let inventoryId = EntityId.newId () |> InventoryId.create

        let inventoryName =
            "New Inventory"
            |> MediumString.create "InventoryName"
            |> Result.defaultWith (fun _ -> failwith "Failed to create inventory name")
            |> InventoryName.create

        let streamId = inventoryId |> InventoryEventStreamId.fromInventoryId

        use! eventStreamSession = eventStore.Open<InventoryEvent, InventoryState>(streamId, eventMapper)

        let domainEvents =
            [ { InventoryCreated.InventoryId = inventoryId
                Name = inventoryName
                IsActive = true } ]

        let eventsWithMetadata =
            domainEvents
            |> List.map (fun e ->
                let genericEvent = e :> obj

                { EventWithMetadata.Event = genericEvent
                  EventType = genericEvent.GetType()
                  Metadata =
                    Some
                        { EventId = EventId.newId ()
                          CorrelationId = EventId.newId ()
                          CausationId = EventId.newId () } })

        do! eventStreamSession.AppendEventsWithMetadata(eventsWithMetadata)

    // No explicit assert here - in case of an error AppendEventsWithMetadata will raise an exception
    }

[<Fact>]
let ``EventStoreSession GetAllEvents concatenates existing events and new events`` () =
    task {
        let inventoryId = EntityId.newId () |> InventoryId.create

        let inventoryName =
            "Inventory"
            |> MediumString.create "InventoryName"
            |> Result.defaultWith (fun _ -> failwith "Failed to create inventory name")
            |> InventoryName.create

        let inventoryNewName =
            "New Inventory"
            |> MediumString.create "InventoryName"
            |> Result.defaultWith (fun _ -> failwith "Failed to create inventory name")
            |> InventoryName.create

        let streamId = inventoryId |> InventoryEventStreamId.fromInventoryId

        // set up existing events
        use! existingEventStreamSession = eventStore.Open<InventoryEvent, InventoryState>(streamId, eventMapper)

        let existingDomainEvents =
            [ InventoryEvent.InventoryCreated
                  { InventoryCreated.InventoryId = inventoryId
                    Name = inventoryName
                    IsActive = true } ]

        let existingGenericEvents = existingDomainEvents |> List.map (fun e -> e :> obj)

        do! existingEventStreamSession.AppendEvents(existingGenericEvents)

        do! eventStore.Save(existingEventStreamSession)

        // set up new events
        use! newEventStreamSession = eventStore.Open<InventoryEvent, InventoryState>(streamId, eventMapper)

        let newDomainEvents =
            [ InventoryEvent.InventoryRenamed
                  { InventoryRenamed.InventoryId = inventoryId
                    OldName = inventoryName
                    NewName = inventoryNewName } ]

        let newGenericEvents = newDomainEvents |> List.map (fun e -> e :> obj)

        do! newEventStreamSession.AppendEvents(newGenericEvents)

        // get all events
        let! allEvents = newEventStreamSession.GetAllEvents()

        allEvents.Events |> Seq.length |> should equal 2
    }

module CQRS.Application.Tests.EventStoreTests

open CQRS.Adapters.EventStore
open CQRS.Application
open CQRS.Domain.Inventory
open CQRS.Domain.ValueTypes
open CQRS.EntityIds
open CQRS.Ports.EventStore
open FsUnit
open Xunit

let testInventoryName (name: string) : InventoryName =
    name
    |> MediumString.create "InventoryName"
    |> Result.defaultWith (fun _ -> failwith "Internal error")
    |> InventoryName.create

let private getRandomStreamId () =
    EntityId.newId () |> EntityId.value |> EventStreamId.create

let private eventMapper = InventoryEventStreamDtoMapper()

let private eventStore =
    new InMemoryEventStore(SystemTextJsonEventSerializer(), None) :> IEventStore

[<Fact>]
let ``EventStore Open can open a new stream`` () =
    task {
        let streamId = getRandomStreamId ()
        use! eventStreamSession = eventStore.Open<InventoryEvent, InventoryState>(streamId, eventMapper)
        let! stream = eventStreamSession.GetAllEvents()

        stream.StreamId |> should equal streamId
        stream.StreamVersion |> should equal 0L
    }

[<Fact>]
let ``EventStore Save can save a new stream`` () =
    task {
        let streamId = getRandomStreamId ()
        use! eventStreamSession = eventStore.Open<InventoryEvent, InventoryState>(streamId, eventMapper)
        do! eventStore.Save(eventStreamSession)

        let! exists = eventStore.Contains streamId
        exists |> should be True
    }

[<Fact>]
let ``EventStore GetStream can open an existing stream`` () =
    task {
        let streamId = getRandomStreamId ()
        use! eventStreamSession = eventStore.Open<InventoryEvent, InventoryState>(streamId, eventMapper)
        do! eventStore.Save(eventStreamSession)
        use! eventStreamSession2 = eventStore.Open<InventoryEvent, InventoryState>(streamId, eventMapper)

        let! stream = eventStreamSession2.GetAllEvents()
        stream.StreamId |> should equal streamId
    }

[<Fact>]
let ``EventStore Save can save an existing stream`` () =
    task {
        let streamId = getRandomStreamId ()
        use! eventStreamSession = eventStore.Open<InventoryEvent, InventoryState>(streamId, eventMapper)
        do! eventStore.Save(eventStreamSession)
        use! eventStreamSession2 = eventStore.Open<InventoryEvent, InventoryState>(streamId, eventMapper)
        do! eventStore.Save(eventStreamSession2)

        let! exists = eventStore.Contains streamId
        exists |> should be True
    }

[<Fact>]
let ``EventStore Delete can delete existing stream`` () =
    task {
        let streamId = getRandomStreamId ()
        use! eventStreamSession = eventStore.Open<InventoryEvent, InventoryState>(streamId, eventMapper)
        do! eventStore.Save(eventStreamSession)

        let! removed = eventStore.Delete streamId
        removed |> should be True

        let! exists = eventStore.Contains streamId
        exists |> should be False
    }

[<Fact>]
let ``EventStore Delete can handle non-existing stream`` () =
    task {
        let streamId = getRandomStreamId ()
        let! removed = eventStore.Delete streamId
        removed |> should be False

        let! exists = eventStore.Contains streamId
        exists |> should be False
    }

[<Fact>]
let ``EventStore Contains can handle existing streams`` () =
    task {
        let streamId = getRandomStreamId ()
        use! eventStreamSession = eventStore.Open<InventoryEvent, InventoryState>(streamId, eventMapper)
        do! eventStore.Save(eventStreamSession)

        let! exists = eventStore.Contains streamId
        exists |> should be True
    }

[<Fact>]
let ``EventStore Contains can handle non-existing streams`` () =
    task {
        let streamId = getRandomStreamId ()

        let! exists = eventStore.Contains streamId
        exists |> should be False
    }

[<Fact>]
let ``EventStore Contains does not take into account new streams that have not been saved yet`` () =
    task {
        let streamId = getRandomStreamId ()
        use! _x = eventStore.Open<InventoryEvent, InventoryState>(streamId, eventMapper)

        let! exists = eventStore.Contains streamId
        exists |> should be False
    }

namespace CQRS.Application

open CQRS.Domain
open CQRS.Domain.Inventory
open CQRS.Ports.EventStore

type InventoryEventStreamProjection() =
    let getIdFromStreamId (streamId: EventStreamId) : InventoryId =
        streamId
        |> EventStreamId.value
        |> InventoryId.fromString "InventoryId"
        |> Result.defaultWith (fun _ -> failwith "Failed to create InventoryId")

    interface IEventStreamProjection<InventoryEvent, InventoryState> with
        member _.GetInitialState(eventStreamId) =
            Uninitialized <| getIdFromStreamId eventStreamId

        member _.Apply(state, evt) =
            evt |> InventoryStateProjection.apply state

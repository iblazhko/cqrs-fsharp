namespace CQRS.Application

open CQRS.Domain
open CQRS.Domain.Inventory
open CQRS.Ports.EventStore

type InventoryEventStreamProjection() =
    interface IEventStreamProjection<InventoryEvent, InventoryState> with
        member _.GetInitialState(eventStreamId) =
            eventStreamId
            |> EventStreamId.value
            |> InventoryId.fromString "InventoryId"
            |> Result.map Uninitialized
            |> Result.mapError (fun e -> $"%A{e}")

        member _.Apply(state, evt) =
            evt |> InventoryStateProjection.apply state

module CQRS.Application.InventoryItemEventStoreProjection

open CQRS.Domain
open CQRS.Domain.Inventory
open CQRS.Domain.ValueTypes
open CQRS.Ports.EventStore
open FsToolkit.ErrorHandling

type InventoryItemEventStreamProjection() =
    let newInventoryItemId (streamId: EventStreamId) : InventoryItemId =
        let idResult =
            streamId |> EventStreamId.value |> InventoryItemId.fromString "InventoryItemId"

        match idResult with
        | Ok x -> x
        | Error _ -> failwith "Failed to create InventoryItemId"

    let newInventoryItemName () : InventoryItemName =
        let nameResult =
            result {
                let! ms = "N/A" |> MediumString.create "InventoryItemName"
                return ms |> InventoryItemName.create
            }

        match nameResult with
        | Ok x -> x
        | Error _ -> failwith "Failed to create InventoryItemName"

    interface IEventStreamProjection<InventoryItemEvent, InventoryItemState> with
        member this.GetInitialState(eventStreamId) =
            { InventoryItemId = newInventoryItemId eventStreamId
              Name = newInventoryItemName ()
              StockQuantity = StockQuantity.Empty
              IsNew = true
              IsActive = true }

        member this.Apply(state, evt) =
            evt |> InventoryEventsApplier.apply state

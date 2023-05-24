namespace CQRS.Application

open CQRS.Domain
open CQRS.Domain.Inventory
open CQRS.Domain.ValueTypes
open CQRS.Ports.EventStore
open FsToolkit.ErrorHandling

type EventStoreInventorySateProjection() =
    let getIdFromStreamId (streamId: EventStreamId) : InventoryId =
        let idResult =
            streamId |> EventStreamId.value |> InventoryId.fromString "InventoryId"

        match idResult with
        | Ok x -> x
        | Error _ -> failwith "Failed to create InventoryId"

    let newInventoryName () : InventoryName =
        let nameResult =
            result {
                let! ms = "N/A" |> MediumString.create "InventoryName"
                return ms |> InventoryName.create
            }

        match nameResult with
        | Ok x -> x
        | Error _ -> failwith "Failed to create InventoryName"

    interface IEventStreamProjection<InventoryEvent, InventoryState> with
        member this.GetInitialState(eventStreamId) =
            { InventoryId = getIdFromStreamId eventStreamId
              Name = newInventoryName ()
              StockQuantity = StockQuantity.Empty
              IsNew = true
              IsActive = true }

        member this.Apply(state, evt) =
            evt |> InventoryStateProjection.apply state

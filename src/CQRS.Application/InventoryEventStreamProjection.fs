namespace CQRS.Application

open CQRS.Domain
open CQRS.Domain.Inventory
open CQRS.Domain.ValueTypes
open CQRS.Ports.EventStore
open FsToolkit.ErrorHandling

type InventoryEventStreamProjection() =
    let getIdFromStreamId (streamId: EventStreamId) : InventoryId =
        streamId
        |> EventStreamId.value
        |> InventoryId.fromString "InventoryId"
        |> Result.defaultWith (fun _ -> failwith "Failed to create InventoryId")

    let newInventoryName () : InventoryName =
        "N/A"
        |> MediumString.create "InventoryName"
        |> Result.defaultWith (fun _ -> failwith "Failed to create InventoryName")
        |> InventoryName.create

    interface IEventStreamProjection<InventoryEvent, InventoryState> with
        member this.GetInitialState(eventStreamId) =
            { InventoryId = getIdFromStreamId eventStreamId
              Name = newInventoryName ()
              StockQuantity = StockQuantity.Empty
              IsNew = true
              IsActive = true }

        member this.Apply(state, evt) =
            evt |> InventoryStateProjection.apply state

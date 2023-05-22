namespace CQRS.Application

open CQRS.Domain
open CQRS.Domain.Inventory
open CQRS.Ports.EventStore

module InventoryItemEventStreamId =
    let fromInventoryItemId (id: InventoryItemId) =
        InventoryItemId.toString id |> EventStreamId.create

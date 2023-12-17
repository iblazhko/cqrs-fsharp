namespace CQRS.Application

open CQRS.Domain.Inventory
open CQRS.Ports.EventStore

module InventoryEventStreamId =
    let create (id: InventoryId) =
        InventoryId.toString id |> EventStreamId.create

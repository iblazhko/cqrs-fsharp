namespace CQRS.Application

open CQRS.Domain.Inventory
open CQRS.Ports.EventStore

module InventoryEventStreamId =
    let fromInventoryId (id: InventoryId) =
        InventoryId.toString id |> EventStreamId.create

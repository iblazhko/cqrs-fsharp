module CQRS.Projections.InventoryItemProjectionDocumentId

open CQRS.Domain.Inventory
open CQRS.Ports.ProjectionStore

let fromInventoryItemId (id: InventoryItemId) =
    InventoryItemId.toString id |> DocumentId.create

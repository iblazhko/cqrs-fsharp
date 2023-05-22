module CQRS.Projections.InventoryItemProjectionDocumentId

open CQRS.Domain.Inventory
open CQRS.EntityIds
open CQRS.Ports.ProjectionStore

let fromInventoryItemId (id: InventoryItemId) =
    InventoryItemId.value id |> EntityId.toString |> DocumentId.create

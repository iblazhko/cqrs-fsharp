module CQRS.Projections.InventoryProjectionDocumentId

open CQRS.Domain.Inventory
open CQRS.EntityIds
open CQRS.Ports.ProjectionStore

let fromInventoryId (id: InventoryId) =
    id |> InventoryId.value |> EntityId.value |> DocumentId.create

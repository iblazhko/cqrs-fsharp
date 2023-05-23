module CQRS.Projections.InventoryProjectionDocumentId

open CQRS.Domain.Inventory
open CQRS.EntityIds
open CQRS.Ports.ProjectionStore

let fromInventoryId (id: InventoryId) =
    InventoryId.value id |> EntityId.toString |> DocumentId.create

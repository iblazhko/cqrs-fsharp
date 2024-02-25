module CQRS.Projections.Repositories.InventoryViewModelQueryRepository

open CQRS.EntityIds
open CQRS.Ports.ProjectionStore
open CQRS.Projections

let getDocument (projectionStore: IProjectionStore<InventoryViewModel>) inventoryId =
    task {
        use! collection = projectionStore.OpenDocumentCollection(InventoryCollection.InventoryProjectionId)

        let documentId = inventoryId |> EntityId.value |> DocumentId.create
        let! document = collection.GetById(documentId)

        return document
    }

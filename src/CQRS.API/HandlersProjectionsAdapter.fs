module CQRS.API.HandlersProjectionsAdapter

open System.Threading.Tasks
open CQRS.EntityIds
open CQRS.Ports.ProjectionStore
open CQRS.Projections
open FPrimitive
open Serilog


type DocumentQueryResult =
    | Document of InventoryViewModel
    | NotFound
    | BadRequest of ErrorsByTag

let getInventoryViewModel
    (projectionsStore: IProjectionStore<InventoryViewModel>)
    (inventoryId: EntityId)
    : Task<DocumentQueryResult> =
    task {
        // TODO: Use explicit dependency for logging
        Log.Logger.Information("Retrieving inventory {InventoryId}", inventoryId)

        use! collection = projectionsStore.OpenDocumentCollection(InventoryCollection.InventoryProjectionId)

        let documentId = inventoryId |> EntityId.toString |> DocumentId.create
        let! document = collection.GetById(documentId)

        let result =
            match document with
            | Some vm -> Document vm
            | None -> NotFound

        return result
    }

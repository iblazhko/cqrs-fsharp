module CQRS.API.ProjectionHandlers

open System.Threading.Tasks
open CQRS.EntityIds
open CQRS.Ports.ProjectionStore
open CQRS.Projections
open CQRS.Projections.Repositories
open FPrimitive
open Serilog

type DocumentQueryResult =
    | Document of InventoryViewModel
    | NotFound
    | BadRequest of ErrorsByTag

let getInventoryViewModel
    (projectionStore: IProjectionStore<InventoryViewModel>)
    (inventoryId: EntityId)
    : Task<DocumentQueryResult> =
    task {
        // TODO: Use explicit dependency for logging
        Log.Logger.Information("Retrieving inventory {InventoryId}", inventoryId)

        let! document = inventoryId |> InventoryViewModelQueryRepository.getDocument projectionStore

        let result =
            match document with
            | Some vm -> Document vm
            | None -> NotFound

        return result
    }

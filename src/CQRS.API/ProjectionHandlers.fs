module CQRS.API.ProjectionHandlers

open System.Threading.Tasks
open CQRS.Application.CommandProcessingStatusRecording
open CQRS.EntityIds
open CQRS.Ports.Messaging
open CQRS.Ports.ProjectionStore
open CQRS.Projections
open CQRS.Projections.Repositories
open FPrimitive
open Serilog

type DocumentQueryResult<'T> =
    | Document of 'T
    | NotFound
    | BadRequest of ErrorsByTag

let getInventoryViewModel
    (projectionStore: IProjectionStore<InventoryViewModel>)
    (inventoryId: EntityId)
    : Task<DocumentQueryResult<InventoryViewModel>> =
    task {
        Log.Logger.Information("Retrieving inventory {InventoryId}", inventoryId)
        let! document = inventoryId |> InventoryViewModelQueryRepository.getDocument projectionStore
        return
            match document with
            | Some vm -> Document vm
            | None -> NotFound
    }

let getCommandProcessingStatus
    (projectionStore: IProjectionStore<CommandProcessingStatusViewModel>)
    (commandId: MessagingId)
    : Task<DocumentQueryResult<CommandProcessingStatusViewModel>> =
    task {
        Log.Logger.Information("Retrieving command processing status {CommandId}", commandId)
        let! result = GetCommandProcessingStatusQueryRepository.getDocument projectionStore commandId
        return
            match result with
            | Some vm -> Document vm
            | None -> NotFound
    }

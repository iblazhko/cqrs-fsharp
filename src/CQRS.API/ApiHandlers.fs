module CQRS.API.ApiHandlers

open System
open System.Net
open System.Text.Json
open System.Threading.Tasks
open CQRS.Application.CommandProcessingStatusRecording
open Microsoft.AspNetCore.Http
open FPrimitive
open CQRS.EntityIds
open CQRS.Ports.ProjectionStore
open CQRS.Ports.Messaging
open CQRS.Projections
open CQRS.DTO.V1

open type TypedResults

type ApiResult<'T> =
    | Success of 'T
    | ValidationError of ErrorsByTag
    | OperationError of ErrorsByTag
    | NotFound

let mapApiResult<'T> (result: ApiResult<'T>) : IResult =
    match result with
    | Success s -> Ok(s) :> IResult
    | ValidationError v -> BadRequest(v) :> IResult
    | OperationError e ->
        Problem(JsonSerializer.Serialize(e), statusCode = int HttpStatusCode.InternalServerError) :> IResult
    | NotFound -> Problem(statusCode = int HttpStatusCode.NotFound) :> IResult

let private mapCommandOperationResult result =
    match result with
    | Ok success -> Success success
    | Error error -> OperationError error

let private mapQueryOperationResult result =
    match result with
    | ProjectionHandlers.DocumentQueryResult.Document vm -> Success vm
    | ProjectionHandlers.DocumentQueryResult.NotFound -> NotFound
    | ProjectionHandlers.DocumentQueryResult.BadRequest error -> OperationError error

let private validatingApiHandler validationResult operation mapper =
    task {
        let! apiResult =
            task {
                match validationResult with
                | Ok validInput ->
                    let! result = validInput |> operation
                    return result |> mapper
                | Error error -> return ValidationError error
            }

        return apiResult
    }

let private validatingCommandApiHandler validationResult operation =
    validatingApiHandler validationResult operation mapCommandOperationResult

let private validatingQueryApiHandler validationResult operation =
    validatingApiHandler validationResult operation mapQueryOperationResult

module CommandApiHandlers =
    let createInventory
        (messageBus: IMessageBus)
        (clock: TimeProvider)
        (cmd: CreateInventoryCommand)
        : Task<ApiResult<AcceptedResponse>> =
        task {
            let! result = cmd |> MessageBusHandlers.createInventory messageBus clock
            return result |> mapCommandOperationResult
        }

    let renameInventory
        (messageBus: IMessageBus)
        (clock: TimeProvider)
        (id: string, name: string)
        : Task<ApiResult<AcceptedResponse>> =
        validatingCommandApiHandler (id |> EntityId.create "InventoryId") (fun inventoryId ->
            let cmd = RenameInventoryCommand()
            cmd.InventoryId <- inventoryId |> EntityId.value
            cmd.NewName <- name
            cmd |> MessageBusHandlers.renameInventory messageBus clock)

    let addItemsToInventory
        (messageBus: IMessageBus)
        (clock: TimeProvider)
        (id: string, count: int)
        : Task<ApiResult<AcceptedResponse>> =
        validatingCommandApiHandler (id |> EntityId.create "InventoryId") (fun inventoryId ->
            let cmd = AddItemsToInventoryCommand()
            cmd.InventoryId <- inventoryId |> EntityId.value
            cmd.Count <- count
            cmd |> MessageBusHandlers.addItemsToInventory messageBus clock)


    let removeItemsFromInventory
        (messageBus: IMessageBus)
        (clock: TimeProvider)
        (id: string, count: int)
        : Task<ApiResult<AcceptedResponse>> =
        validatingCommandApiHandler (id |> EntityId.create "InventoryId") (fun inventoryId ->
            let cmd = RemoveItemsFromInventoryCommand()
            cmd.InventoryId <- inventoryId |> EntityId.value
            cmd.Count <- count
            cmd |> MessageBusHandlers.removeItemsFromInventory messageBus clock)


    let deactivateInventory
        (messageBus: IMessageBus)
        (clock: TimeProvider)
        (id: string)
        : Task<ApiResult<AcceptedResponse>> =
        validatingCommandApiHandler (id |> EntityId.create "InventoryId") (fun inventoryId ->
            let cmd = DeactivateInventoryCommand()
            cmd.InventoryId <- inventoryId |> EntityId.value
            cmd |> MessageBusHandlers.deactivateInventory messageBus clock)

module QueryApiHandlers =
    let getInventory
        (projectionStore: IProjectionStore<InventoryViewModel>)
        (id: string)
        : Task<ApiResult<InventoryViewModel>> =
        validatingQueryApiHandler (id |> EntityId.create "InventoryId") (fun inventoryId ->
            inventoryId |> ProjectionHandlers.getInventoryViewModel projectionStore)

    let getCommandProcessingStatus
        (projectionStore: IProjectionStore<CommandProcessingStatusViewModel>)
        (id: string)
        : Task<ApiResult<CommandProcessingStatusViewModel>> =
        task {
            let! result = MessagingId(id) |> ProjectionHandlers.getCommandProcessingStatus projectionStore
            return
                match result with
                | ProjectionHandlers.DocumentQueryResult.Document vm -> ApiResult.Success vm
                | ProjectionHandlers.DocumentQueryResult.NotFound -> ApiResult.NotFound
                | ProjectionHandlers.DocumentQueryResult.BadRequest x -> ApiResult.ValidationError x
        }

module CQRS.API.ApiRoutes

open System
open System.Net
open System.Text.Json
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open FPrimitive
open CQRS.EntityIds
open CQRS.Ports.ProjectionStore
open CQRS.Ports.Messaging
open CQRS.Ports.Time
open CQRS.Projections
open CQRS.DTO.V1

open type TypedResults

type private ApiResult<'T> =
    | Success of 'T
    | ValidationError of ErrorsByTag
    | OperationError of ErrorsByTag
    | NotFound

let private mapApiResult<'T> (result: ApiResult<'T>) : IResult =
    match result with
    | Success s -> Ok(s) :> IResult
    | ValidationError v -> BadRequest(v) :> IResult
    | OperationError e -> (Problem(JsonSerializer.Serialize(e), statusCode = int HttpStatusCode.BadRequest) :> IResult)
    | NotFound -> (Problem(statusCode = int HttpStatusCode.NotFound) :> IResult)

module private ApiRouteHandlers =
    let createInventory
        (cmd: CreateInventoryCommand)
        (messageBus: IMessageBus)
        (clock: IClock)
        : Task<ApiResult<AcceptedResponse>> =
        task {
            let! result = cmd |> HandlersMessageBusAdapter.createInventory messageBus clock

            match result with
            | Ok success -> return Success success
            | Error error -> return OperationError error
        }

    let renameInventory
        (id: string)
        (name: string)
        (messageBus: IMessageBus)
        (clock: IClock)
        : Task<ApiResult<AcceptedResponse>> =
        task {
            let inventoryIdResult = id |> EntityId.fromString "InventoryId"

            match inventoryIdResult with
            | Ok inventoryId ->
                let cmd = RenameInventoryCommand()
                cmd.InventoryId <- inventoryId |> EntityId.value
                cmd.NewName <- name
                let! result = cmd |> HandlersMessageBusAdapter.renameInventory messageBus clock

                match result with
                | Ok success -> return Success success
                | Error error -> return OperationError error
            | Error error -> return ValidationError error
        }

    let addItemsToInventory
        (id: string)
        (count: int)
        (messageBus: IMessageBus)
        (clock: IClock)
        : Task<ApiResult<AcceptedResponse>> =
        task {
            let inventoryIdResult = id |> EntityId.fromString "InventoryId"

            match inventoryIdResult with
            | Ok inventoryId ->
                let cmd = AddItemsToInventoryCommand()
                cmd.InventoryId <- inventoryId |> EntityId.value
                cmd.Count <- count

                let! result = cmd |> HandlersMessageBusAdapter.addItemsToInventory messageBus clock

                match result with
                | Ok success -> return Success success
                | Error error -> return OperationError error
            | Error error -> return ValidationError error
        }

    let removeItemsFromInventory
        (id: string)
        (count: int)
        (messageBus: IMessageBus)
        (clock: IClock)
        : Task<ApiResult<AcceptedResponse>> =
        task {
            let inventoryIdResult = id |> EntityId.fromString "InventoryId"

            match inventoryIdResult with
            | Ok inventoryId ->
                let cmd = RemoveItemsFromInventoryCommand()
                cmd.InventoryId <- inventoryId |> EntityId.value
                cmd.Count <- count

                let! result = cmd |> HandlersMessageBusAdapter.removeItemsFromInventory messageBus clock

                match result with
                | Ok success -> return Success success
                | Error error -> return OperationError error
            | Error error -> return ValidationError error
        }

    let deactivateInventory (id: string) (messageBus: IMessageBus) (clock: IClock) : Task<ApiResult<AcceptedResponse>> =
        task {
            let inventoryIdResult = id |> EntityId.fromString "InventoryId"

            match inventoryIdResult with
            | Ok inventoryId ->
                let cmd = DeactivateInventoryCommand()
                cmd.InventoryId <- inventoryId |> EntityId.value

                let! result = cmd |> HandlersMessageBusAdapter.deactivateInventory messageBus clock

                match result with
                | Ok success -> return Success success
                | Error error -> return OperationError error
            | Error error -> return ValidationError error
        }

    let getInventory
        (id: string)
        (projectionStore: IProjectionStore<InventoryViewModel>)
        : Task<ApiResult<InventoryViewModel>> =
        task {
            let inventoryIdResult = id |> EntityId.fromString "InventoryId"

            match inventoryIdResult with
            | Ok inventoryId ->
                let! result = inventoryId |> HandlersProjectionsAdapter.getInventoryViewModel projectionStore

                match result with
                | HandlersProjectionsAdapter.DocumentQueryResult.Document vm -> return Success vm
                | HandlersProjectionsAdapter.DocumentQueryResult.NotFound -> return NotFound
                | HandlersProjectionsAdapter.DocumentQueryResult.BadRequest error -> return OperationError error
            | Error error -> return ValidationError error
        }

let configureApiRoutes (app: WebApplication) =

    app.MapGet("/v1/hello", Func<Task<IResult>>(fun () -> task { return Ok("Hi there!") }))
    |> ignore

    app.MapPost(
        "/v1/inventories",
        Func<CreateInventoryCommand, IMessageBus, IClock, Task<IResult>>
            (fun (cmd: CreateInventoryCommand) (messageBus: IMessageBus) (clock: IClock) ->
                task {
                    let! result = ApiRouteHandlers.createInventory cmd messageBus clock
                    return (mapApiResult result)
                })
    )
    |> ignore

    app.MapPost(
        "/v1/inventories/{id}/rename/{name}",
        Func<string, string, IMessageBus, IClock, Task<IResult>>
            (fun (id: string) (name: string) (messageBus: IMessageBus) (clock: IClock) ->
                task {
                    let! result = ApiRouteHandlers.renameInventory id name messageBus clock
                    return (mapApiResult result)
                })
    )
    |> ignore

    app.MapPost(
        "/v1/inventories/{id}/add/{count}",
        Func<string, int, IMessageBus, IClock, Task<IResult>>
            (fun (id: string) (count: int) (messageBus: IMessageBus) (clock: IClock) ->
                task {
                    let! result = ApiRouteHandlers.addItemsToInventory id count messageBus clock
                    return (mapApiResult result)
                })
    )
    |> ignore

    app.MapPost(
        "/v1/inventories/{id}/remove/{count}",
        Func<string, int, IMessageBus, IClock, Task<IResult>>
            (fun (id: string) (count: int) (messageBus: IMessageBus) (clock: IClock) ->
                task {
                    let! result = ApiRouteHandlers.removeItemsFromInventory id count messageBus clock
                    return (mapApiResult result)
                })
    )
    |> ignore

    app.MapPost(
        "/v1/inventories/{id}/deactivate",
        Func<string, IMessageBus, IClock, Task<IResult>>(fun (id: string) (messageBus: IMessageBus) (clock: IClock) ->
            task {
                let! result = ApiRouteHandlers.deactivateInventory id messageBus clock
                return (mapApiResult result)
            })
    )
    |> ignore

    app.MapGet(
        "/v1/inventories/{id}",
        Func<string, IProjectionStore<InventoryViewModel>, Task<IResult>>
            (fun (id: string) (projectionStore: IProjectionStore<InventoryViewModel>) ->
                task {
                    let! result = ApiRouteHandlers.getInventory id projectionStore
                    return (mapApiResult result)
                })
    )
    |> ignore

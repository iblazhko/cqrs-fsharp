module CQRS.API.Host.ApiRoutes

open System
open System.Net
open System.Text.Json
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open CQRS.EntityIds
open CQRS.Ports.ProjectionStore
open CQRS.Projections
open CQRS.DTO.V1
open CQRS.Ports.Messaging

open CQRS.API.HandlersMessageBusAdapter
open CQRS.API.HandlersProjectionsAdapter

open type TypedResults

let systemClock = fun () -> DateTimeOffset.Now

let configureApiRoutes (app: WebApplication) =

    app.MapGet("/v1/hello", Func<Task<IResult>>(fun () -> task { return Ok("Hi there!") }))
    |> ignore

    app.MapPost(
        "/v1/inventories",
        Func<CreateInventoryCommand, IMessageBus, Task<IResult>>
            (fun (cmd: CreateInventoryCommand) (messageBus: IMessageBus) ->
                task {
                    let! result = cmd |> createInventory messageBus systemClock

                    match result with
                    | Ok success -> return (Ok(success) :> IResult)
                    | Error error ->
                        return
                            (Problem(
                                JsonSerializer.Serialize(error),
                                statusCode = int HttpStatusCode.InternalServerError
                            )
                            :> IResult)
                })
    )
    |> ignore

    app.MapPost(
        "/v1/inventories/{id}/rename/{name}",
        Func<string, string, IMessageBus, Task<IResult>>(fun (id: string) (name: string) (messageBus: IMessageBus) ->
            task {
                // TODO: translate error to BadRequest
                let inventoryId =
                    id
                    |> EntityId.fromString "InventoryId"
                    |> Result.defaultWith (fun _ -> failwith "Invalid entityId")

                let cmd = RenameInventoryCommand()
                cmd.InventoryId <- inventoryId |> EntityId.value
                cmd.NewName <- name

                let! result = cmd |> renameInventory messageBus systemClock

                match result with
                | Ok success -> return (Ok(success) :> IResult)
                | Error error ->
                    return
                        (Problem(JsonSerializer.Serialize(error), statusCode = int HttpStatusCode.InternalServerError)
                        :> IResult)
            })
    )
    |> ignore

    app.MapPost(
        "/v1/inventories/{id}/add/{count}",
        Func<string, int, IMessageBus, Task<IResult>>(fun (id: string) (count: int) (messageBus: IMessageBus) ->
            task {
                // TODO: translate error to BadRequest
                let inventoryId =
                    id
                    |> EntityId.fromString "InventoryId"
                    |> Result.defaultWith (fun _ -> failwith "Invalid entityId")


                let cmd = AddItemsToInventoryCommand()
                cmd.InventoryId <- inventoryId |> EntityId.value
                cmd.Count <- count

                let! result = cmd |> addItemsToInventory messageBus systemClock

                match result with
                | Ok success -> return (Ok(success) :> IResult)
                | Error error ->
                    return
                        (Problem(JsonSerializer.Serialize(error), statusCode = int HttpStatusCode.InternalServerError)
                        :> IResult)
            })
    )
    |> ignore

    app.MapPost(
        "/v1/inventories/{id}/remove/{count}",
        Func<string, int, IMessageBus, Task<IResult>>(fun (id: string) (count: int) (messageBus: IMessageBus) ->
            task {
                // TODO: translate error to BadRequest
                let inventoryId =
                    id
                    |> EntityId.fromString "InventoryId"
                    |> Result.defaultWith (fun _ -> failwith "Invalid entityId")

                let cmd = RemoveItemsFromInventoryCommand()
                cmd.InventoryId <- inventoryId |> EntityId.value
                cmd.Count <- count

                let! result = cmd |> removeItemsFromInventory messageBus systemClock

                match result with
                | Ok success -> return (Ok(success) :> IResult)
                | Error error ->
                    return
                        (Problem(JsonSerializer.Serialize(error), statusCode = int HttpStatusCode.InternalServerError)
                        :> IResult)
            })
    )
    |> ignore

    app.MapPost(
        "/v1/inventories/{id}/deactivate",
        Func<string, IMessageBus, Task<IResult>>(fun (id: string) (messageBus: IMessageBus) ->
            task {
                // TODO: translate error to BadRequest
                let inventoryId =
                    id
                    |> EntityId.fromString "InventoryId"
                    |> Result.defaultWith (fun _ -> failwith "Invalid entityId")


                let cmd = DeactivateInventoryCommand()
                cmd.InventoryId <- inventoryId |> EntityId.value

                let! result = cmd |> deactivateInventory messageBus systemClock

                match result with
                | Ok success -> return (Ok(success) :> IResult)
                | Error error ->
                    return
                        (Problem(JsonSerializer.Serialize(error), statusCode = int HttpStatusCode.InternalServerError)
                        :> IResult)
            })
    )
    |> ignore

    app.MapGet(
        "/v1/inventories/{id}",
        Func<string, IProjectionStore<InventoryViewModel>, Task<IResult>>
            (fun (id: string) (projectionStore: IProjectionStore<InventoryViewModel>) ->
                task {
                    // TODO: translate error to BadRequest
                    let inventoryId =
                        id
                        |> EntityId.fromString "InventoryId"
                        |> Result.defaultWith (fun _ -> failwith "Invalid entityId")

                    let! result = inventoryId |> getInventoryViewModel projectionStore

                    match result with
                    | Document vm -> return (Ok(vm) :> IResult)
                    | NotFound -> return (Problem(statusCode = int HttpStatusCode.NotFound) :> IResult)
                    | BadRequest errors ->
                        return
                            (Problem(JsonSerializer.Serialize(errors), statusCode = int HttpStatusCode.BadRequest)
                            :> IResult)
                })
    )
    |> ignore

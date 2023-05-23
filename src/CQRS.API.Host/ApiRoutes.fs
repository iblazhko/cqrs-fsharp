module CQRS.API.Host.ApiRoutes

open System
open System.Net
open CQRS.EntityIds
open CQRS.Ports.ProjectionStore
open CQRS.Projections
open FSharp.MinimalApi
open Microsoft.AspNetCore.Http
open CQRS.API.Handlers
open CQRS.DTO.V1
open CQRS.Ports.Messaging

let systemClock = fun () -> DateTimeOffset.Now

let routes =
    endpoints {
        get "/v1/hello" (fun () -> "Hi there")

        put "/v1/inventories" (fun (cmd: CreateInventoryCommand) (messageBus: IMessageBus) ->
            task {
                let! result = cmd |> createInventory messageBus systemClock

                match result with
                | Ok success -> return (TypedResults.Ok(success) :> IResult)
                | Error error ->
                    return
                        (TypedResults.Problem(
                            System.Text.Json.JsonSerializer.Serialize(error),
                            statusCode = (int) HttpStatusCode.InternalServerError
                        )
                        :> IResult)
            })

        post "/v1/inventories/{id}/add/{count}" (fun (id: string) (count: int) (messageBus: IMessageBus) ->
            task {
                // TODO: translate error to BadRequest
                let inventoryId =
                    id
                    |> EntityId.fromString "InventoryId"
                    |> Result.defaultWith (fun e -> failwith "Invalid entityId")


                let cmd = AddItemsToInventoryCommand()
                cmd.InventoryId <- inventoryId |> EntityId.value
                cmd.Count <- count

                let! result = cmd |> addItemsToInventory messageBus systemClock

                match result with
                | Ok success -> return (TypedResults.Ok(success) :> IResult)
                | Error error ->
                    return
                        (TypedResults.Problem(
                            System.Text.Json.JsonSerializer.Serialize(error),
                            statusCode = (int) HttpStatusCode.InternalServerError
                        )
                        :> IResult)
            })

        post "/v1/inventories/{id}/remove/{count}" (fun (id: string) (count: int) (messageBus: IMessageBus) ->
            task {
                // TODO: translate error to BadRequest
                let inventoryId =
                    id
                    |> EntityId.fromString "InventoryId"
                    |> Result.defaultWith (fun e -> failwith "Invalid entityId")


                let cmd = RemoveItemsFromInventoryCommand()
                cmd.InventoryId <- inventoryId |> EntityId.value
                cmd.Count <- count

                let! result = cmd |> removeItemsFromInventory messageBus systemClock

                match result with
                | Ok success -> return (TypedResults.Ok(success) :> IResult)
                | Error error ->
                    return
                        (TypedResults.Problem(
                            System.Text.Json.JsonSerializer.Serialize(error),
                            statusCode = (int) HttpStatusCode.InternalServerError
                        )
                        :> IResult)
            })

        post "/v1/inventories/{id}/deactivate" (fun (id: string) (messageBus: IMessageBus) ->
            task {
                // TODO: translate error to BadRequest
                let inventoryId =
                    id
                    |> EntityId.fromString "InventoryId"
                    |> Result.defaultWith (fun e -> failwith "Invalid entityId")


                let cmd = DeactivateInventoryCommand()
                cmd.InventoryId <- inventoryId |> EntityId.value

                let! result = cmd |> deactivateInventory messageBus systemClock

                match result with
                | Ok success -> return (TypedResults.Ok(success) :> IResult)
                | Error error ->
                    return
                        (TypedResults.Problem(
                            System.Text.Json.JsonSerializer.Serialize(error),
                            statusCode = (int) HttpStatusCode.InternalServerError
                        )
                        :> IResult)
            })

        get "/v1/inventories/{id}" (fun (id: string) (projectionStore: IProjectionStore<InventoryViewModel>) ->
            task {
                // TODO: translate error to BadRequest
                let inventoryId =
                    id
                    |> EntityId.fromString "InventoryId"
                    |> Result.defaultWith (fun e -> failwith "Invalid entityId")

                let! result = inventoryId |> getInventoryViewModel projectionStore

                match result with
                | Document vm -> return (TypedResults.Ok(vm) :> IResult)
                | NotFound -> return (TypedResults.Problem(statusCode = int HttpStatusCode.NotFound) :> IResult)
                | BadRequest errors ->
                    return
                        (TypedResults.Problem(
                            System.Text.Json.JsonSerializer.Serialize(errors),
                            statusCode = int HttpStatusCode.BadRequest
                        )
                        :> IResult)
            })

    }

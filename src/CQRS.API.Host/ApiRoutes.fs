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

        put "/v1/inventories" (fun (inventoryItem: CreateInventoryItemCommand) (messageBus: IMessageBus) ->
            task {
                let! result = inventoryItem |> createInventoryItem messageBus systemClock

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

        get "/v1/inventories/{id}" (fun (id: string) (projectionStore: IProjectionStore<InventoryItemViewModel>) ->
            task {
                // TODO: translate error to BadRequest
                let inventoryItemId =
                    id
                    |> EntityId.fromString "InventoryItemId"
                    |> Result.defaultWith (fun e -> failwith "Invalid entityId")

                let! result = inventoryItemId |> getInventoryItem projectionStore

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

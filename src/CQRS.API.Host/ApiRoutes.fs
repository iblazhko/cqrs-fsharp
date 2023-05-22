module CQRS.API.Host.ApiRoutes

open System
open System.Net
open FSharp.MinimalApi
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open CQRS.API.Handlers
open CQRS.DTO.V1
open CQRS.Ports.Messaging

let systemClock = fun () -> DateTimeOffset.Now

let routes =
    endpoints {
        get "/v1/hello" (fun () -> "Hi there")

        put
            "/v1/inventories"
            (fun
                (inventoryItem: CreateInventoryItemCommand)
                (messageBus: IMessageBus)
                (logger: ILogger<CreateInventoryItemCommand>) ->
                task {
                    let! result = inventoryItem |> createInventoryItem messageBus systemClock logger

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
    }

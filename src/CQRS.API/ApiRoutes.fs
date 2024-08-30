module CQRS.API.ApiRoutes

open System
open System.Threading.Tasks
open CQRS.Application.CommandProcessingStatusRecording
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open CQRS.Ports.ProjectionStore
open CQRS.Ports.Messaging
open CQRS.Projections
open CQRS.DTO.V1

open ApiHandlers

let private useApiHandler (handler: unit -> Task<ApiResult<'T>>) =
    task {
        let! result = handler ()
        return result |> mapApiResult
    }

let configureApiRoutes (app: WebApplication) =

    app.MapGet(
        "/v1/hello",
        Func<Task<IResult>>(fun () -> useApiHandler (fun () -> Task.FromResult(Success "Hi there!")))
    )
    |> ignore

    app.MapPost(
        "/v1/inventories",
        Func<CreateInventoryCommand, IMessageBus, TimeProvider, Task<IResult>>
            (fun (cmd: CreateInventoryCommand) (messageBus: IMessageBus) (clock: TimeProvider) ->
                useApiHandler (fun () -> cmd |> CommandApiHandlers.createInventory messageBus clock))
    )
    |> ignore

    app.MapPost(
        "/v1/inventories/{id}/rename/{name}",
        Func<string, string, IMessageBus, TimeProvider, Task<IResult>>
            (fun (id: string) (name: string) (messageBus: IMessageBus) (clock: TimeProvider) ->
                useApiHandler (fun () -> (id, name) |>  CommandApiHandlers.renameInventory messageBus clock))
    )
    |> ignore

    app.MapPost(
        "/v1/inventories/{id}/add/{count}",
        Func<string, int, IMessageBus, TimeProvider, Task<IResult>>
            (fun (id: string) (count: int) (messageBus: IMessageBus) (clock: TimeProvider) ->
                useApiHandler (fun () -> (id, count) |> CommandApiHandlers.addItemsToInventory messageBus clock))
    )
    |> ignore

    app.MapPost(
        "/v1/inventories/{id}/remove/{count}",
        Func<string, int, IMessageBus, TimeProvider, Task<IResult>>
            (fun (id: string) (count: int) (messageBus: IMessageBus) (clock: TimeProvider) ->
                useApiHandler (fun () -> (id, count) |> CommandApiHandlers.removeItemsFromInventory messageBus clock))
    )
    |> ignore

    app.MapPost(
        "/v1/inventories/{id}/deactivate",
        Func<string, IMessageBus, TimeProvider, Task<IResult>>
            (fun (id: string) (messageBus: IMessageBus) (clock: TimeProvider) ->
                useApiHandler (fun () -> id |> CommandApiHandlers.deactivateInventory messageBus clock))
    )
    |> ignore

    app.MapGet(
        "/v1/inventories/{id}",
        Func<string, IProjectionStore<InventoryViewModel>, Task<IResult>>
            (fun (id: string) (projectionStore: IProjectionStore<InventoryViewModel>) ->
                useApiHandler (fun () -> id |> QueryApiHandlers.getInventory projectionStore))
    )
    |> ignore

    app.MapGet(
        "/v1/commands/{id}",
        Func<string, IProjectionStore<CommandProcessingStatusViewModel>, Task<IResult>>
            (fun (id: string) (projectionStore: IProjectionStore<CommandProcessingStatusViewModel>) ->
                useApiHandler (fun () -> id |> QueryApiHandlers.getCommandProcessingStatus projectionStore))
    )
    |> ignore

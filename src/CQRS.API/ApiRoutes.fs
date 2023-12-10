module CQRS.API.ApiRoutes

open System
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open CQRS.Ports.ProjectionStore
open CQRS.Ports.Messaging
open CQRS.Ports.Time
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
        Func<CreateInventoryCommand, IMessageBus, IClock, Task<IResult>>
            (fun (cmd: CreateInventoryCommand) (messageBus: IMessageBus) (clock: IClock) ->
                useApiHandler (fun () -> CommandApiHandlers.createInventory cmd messageBus clock))
    )
    |> ignore

    app.MapPost(
        "/v1/inventories/{id}/rename/{name}",
        Func<string, string, IMessageBus, IClock, Task<IResult>>
            (fun (id: string) (name: string) (messageBus: IMessageBus) (clock: IClock) ->
                useApiHandler (fun () -> CommandApiHandlers.renameInventory id name messageBus clock))
    )
    |> ignore

    app.MapPost(
        "/v1/inventories/{id}/add/{count}",
        Func<string, int, IMessageBus, IClock, Task<IResult>>
            (fun (id: string) (count: int) (messageBus: IMessageBus) (clock: IClock) ->
                useApiHandler (fun () -> CommandApiHandlers.addItemsToInventory id count messageBus clock))
    )
    |> ignore

    app.MapPost(
        "/v1/inventories/{id}/remove/{count}",
        Func<string, int, IMessageBus, IClock, Task<IResult>>
            (fun (id: string) (count: int) (messageBus: IMessageBus) (clock: IClock) ->
                useApiHandler (fun () -> CommandApiHandlers.removeItemsFromInventory id count messageBus clock))
    )
    |> ignore

    app.MapPost(
        "/v1/inventories/{id}/deactivate",
        Func<string, IMessageBus, IClock, Task<IResult>>(fun (id: string) (messageBus: IMessageBus) (clock: IClock) ->
            useApiHandler (fun () -> CommandApiHandlers.deactivateInventory id messageBus clock))
    )
    |> ignore

    app.MapGet(
        "/v1/inventories/{id}",
        Func<string, IProjectionStore<InventoryViewModel>, Task<IResult>>
            (fun (id: string) (projectionStore: IProjectionStore<InventoryViewModel>) ->
                useApiHandler (fun () -> QueryApiHandlers.getInventory id projectionStore))
    )
    |> ignore

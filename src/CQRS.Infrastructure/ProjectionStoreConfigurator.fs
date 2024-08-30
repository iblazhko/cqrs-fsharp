module CQRS.Infrastructure.ProjectionStoreConfigurator

open CQRS.Adapters.ProjectionStore
open CQRS.Application.CommandProcessingStatusRecording
open CQRS.Ports.ProjectionStore
open CQRS.Projections
open Marten
open Microsoft.Extensions.DependencyInjection

let configureServices (services: IServiceCollection) =
    services.AddSingleton<IProjectionStore<InventoryViewModel>>(fun serviceProvider ->
        let martenDocumentStore = serviceProvider.GetRequiredService<IDocumentStore>()

        new MartenDbProjectionStore<InventoryViewModel>(martenDocumentStore) :> IProjectionStore<InventoryViewModel>)
    |> ignore

    services.AddSingleton<IProjectionStore<CommandProcessingStatusViewModel>>(fun serviceProvider ->
        let martenDocumentStore = serviceProvider.GetRequiredService<IDocumentStore>()

        new MartenDbProjectionStore<CommandProcessingStatusViewModel>(martenDocumentStore)
        :> IProjectionStore<CommandProcessingStatusViewModel>)
    |> ignore

    services

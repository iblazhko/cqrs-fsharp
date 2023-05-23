module CQRS.Infrastructure.ProjectionStoreConfigurator

open CQRS.Adapters
open CQRS.Ports.ProjectionStore
open CQRS.Projections
open Marten
open Microsoft.Extensions.DependencyInjection

let configureServices (services: IServiceCollection) =
    services.AddSingleton<IProjectionStore<InventoryItemViewModel>>(fun serviceProvider ->
        let martenDocumentStore = serviceProvider.GetRequiredService<IDocumentStore>()

        new MartenDbProjectionStore<InventoryItemViewModel>(martenDocumentStore)
        :> IProjectionStore<InventoryItemViewModel>)
    |> ignore

    services

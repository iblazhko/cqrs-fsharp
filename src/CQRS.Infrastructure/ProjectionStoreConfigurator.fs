module CQRS.Infrastructure.ProjectionStoreConfigurator

open CQRS.Adapters.ProjectionStore
open CQRS.Ports.ProjectionStore
open CQRS.Projections
open Marten
open Microsoft.Extensions.DependencyInjection

let configureServices (services: IServiceCollection) =
    services.AddSingleton<IProjectionStore<InventoryViewModel>>(fun serviceProvider ->
        let martenDocumentStore = serviceProvider.GetRequiredService<IDocumentStore>()

        new MartenDbProjectionStore<InventoryViewModel>(martenDocumentStore) :> IProjectionStore<InventoryViewModel>)
    |> ignore

    services

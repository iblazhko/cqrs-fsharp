module CQRS.Infrastructure.ProjectionStoreConfigurator

open CQRS.Adapters
open CQRS.Configuration.Infrastructure
open CQRS.Ports.ProjectionStore
open CQRS.Projections
open Marten
open Microsoft.Extensions.DependencyInjection

let configureServices (settings: MartenDbSettings) (services: IServiceCollection) =
    services.AddMarten(fun (options: StoreOptions) -> options.Connection(settings.getConnectionString ()))
    |> ignore

    services.AddSingleton<IProjectionStore<InventoryItemViewModel>>(fun serviceProvider ->
        let martenDocumentStore = serviceProvider.GetRequiredService<IDocumentStore>()

        new MartenDbProjectionStore<InventoryItemViewModel>(martenDocumentStore)
        :> IProjectionStore<InventoryItemViewModel>)
    |> ignore

    services

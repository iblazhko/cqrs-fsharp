module CQRS.Infrastructure.ProjectionStoreConfigurator

open CQRS.Adapters
open CQRS.Configuration.Infrastructure
open CQRS.Ports.ProjectionStore
open CQRS.Projections
open Microsoft.Extensions.DependencyInjection

let configureServices (settings: MartenDbSettings) (services: IServiceCollection) =
    services.AddSingleton<IDocumentStore<InventoryItemViewModel>>(new InMemoryProjectionStore<InventoryItemViewModel>())
    |> ignore

    services

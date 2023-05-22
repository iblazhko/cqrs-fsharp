module CQRS.Infrastructure.EventStoreConfigurator

open CQRS.Adapters
open CQRS.Application
open CQRS.Configuration.Infrastructure
open CQRS.Ports.EventStore
open CQRS.Ports.Messaging
open Marten
open Microsoft.Extensions.DependencyInjection

let configureServices (settings: MartenDbSettings) (services: IServiceCollection) =
    services.AddMarten(fun (options: StoreOptions) -> options.Connection(settings.getConnectionString ()))
    |> ignore

    services.AddSingleton<IEventStore>(fun serviceProvider ->
        // TODO: Replace InMemoryEventStore with MartenDbEventStore

        let bus = serviceProvider.GetRequiredService<IMessageBus>()

        let eventStore =
            (new InMemoryEventStore(SystemTextEventSerializer(), Some(EventStoreEventsPublisher(bus)))) :> IEventStore

        eventStore)
    |> ignore

    services

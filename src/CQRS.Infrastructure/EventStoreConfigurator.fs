module CQRS.Infrastructure.EventStoreConfigurator

open CQRS.Adapters
open CQRS.Application
open CQRS.Ports.EventStore
open CQRS.Ports.Messaging
open Marten
open Microsoft.Extensions.DependencyInjection

let configureServices (services: IServiceCollection) =

    services.AddSingleton<IEventStore>(fun serviceProvider ->
        let martenDocumentStore = serviceProvider.GetRequiredService<IDocumentStore>()
        let bus = serviceProvider.GetRequiredService<IMessageBus>()

        let eventStore =
            (new MartenDbEventStore(martenDocumentStore, Some(EventStoreEventsPublisher(bus)))) :> IEventStore

        eventStore)
    |> ignore

    services

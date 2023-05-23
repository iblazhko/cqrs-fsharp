module CQRS.Infrastructure.MartenDbConfigurator

open CQRS.Configuration.Infrastructure
open Marten
open Marten.Events
open Microsoft.Extensions.DependencyInjection

let configureServices (settings: MartenDbSettings) (services: IServiceCollection) =
    services.AddMarten(fun (options: StoreOptions) ->
        options.Connection(settings.getConnectionString ())
        options.Events.StreamIdentity <- StreamIdentity.AsString)
    |> ignore

    services

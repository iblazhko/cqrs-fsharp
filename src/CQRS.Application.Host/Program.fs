module CQRS.Application.Host.Program

open CQRS.Application.MassTransitConsumers
open CQRS.Configuration
open CQRS.Infrastructure
open CQRS.Infrastructure.Startup
open CQRS.Projections.MassTransitConsumers
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting

[<EntryPoint>]
let main (args: string[]) =
    let settings = CqrsSettingsLoader.getHostSettings "CqrsApp" args
    printStartMessage "CQRS Application Host" settings
    waitForInfrastructure settings

    let builder = WebApplication.CreateBuilder(args)

    let endpointsRegistration =
        { SendConventions = None
          Consumers =
            Some
                { assemblies =
                    [ typedefof<CreateInventoryItemCommandConsumer>.Assembly
                      typedefof<InventoryItemProjectionConsumer>.Assembly ]
                  queuePrefix = "cqrs" } }

    builder.WebHost.ConfigureServices(configureServices endpointsRegistration settings)
    |> ignore

    builder.WebHost.ConfigureLogging(configureLogging settings.Logging) |> ignore
    builder.WebHost.UseUrls(settings.ServiceUrl) |> ignore

    let app = builder.Build()
    app |> configureApp settings

    // Application only exposes healthcheck API endpoint /health (configured by Startup/HealthcheckConfigurator)

    app.Run()
    0

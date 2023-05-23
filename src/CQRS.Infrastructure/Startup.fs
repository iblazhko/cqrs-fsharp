module CQRS.Infrastructure.Startup

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open CQRS.Configuration
open CQRS.Configuration.Infrastructure
open CQRS.Infrastructure.InfrastructureWaitPolicy

let configureServices
    (endpointsRegistration: HostEndpointsRegistration)
    (settings: CqrsHostSettings)
    (services: IServiceCollection)
    =
    services
    |> MartenDbConfigurator.configureServices settings.MartenDb
    |> MessageBusConfigurator.configureServices endpointsRegistration settings.MassTransit
    |> EventStoreConfigurator.configureServices
    |> ProjectionStoreConfigurator.configureServices
    |> HealthcheckConfigurator.configureServices settings
    |> ignore

let configureLogging (settings: LoggingSettings) (builder: ILoggingBuilder) =
    builder |> LoggingConfigurator.configureLogging settings

let configureApp (_: CqrsHostSettings) (app: IApplicationBuilder) =
    app |> HealthcheckConfigurator.configureApp |> ignore

let printStartMessage (hostName: string) (settings: CqrsHostSettings) =
    printfn $"{settings.getStartupInfo hostName}"
    printfn "---------------------"


let waitForInfrastructure (settings: CqrsHostSettings) =
    if
        not (
            waitForInfrastructure
                settings.InfrastructureStartup
                [ { Name = "RabbitMQ"
                    Endpoint = settings.MassTransit.RabbitMq.Endpoint }
                  { Name = "PostgreSQL"
                    Endpoint = settings.MartenDb.Endpoint } ]
        )
    then
        failwith "Infrastructure service(s) not available"

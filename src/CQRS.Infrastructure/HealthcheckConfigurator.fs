module CQRS.Infrastructure.HealthcheckConfigurator

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Diagnostics.HealthChecks
open CQRS.Configuration
open CQRS.Infrastructure

[<Literal>]
let readinessTag = "readiness"

let configureApp (app: IApplicationBuilder) =
    app.UseHealthChecks("/health") |> ignore
    app

let configureServices (settings: CqrsHostSettings) (services: IServiceCollection) =
    services
        .AddHealthChecks()
        .Add(
            HealthCheckRegistration(
                "PostgreSQL",
                PostgresHealthCheck(settings.MartenDb.getConnectionString ()),
                HealthStatus.Degraded,
                [ readinessTag ]
            )
        )
        .Add(
            HealthCheckRegistration(
                "RabbitMQ",
                RabbitMqHealthCheck(settings.MassTransit.RabbitMq.getAmqpUrl ()),
                HealthStatus.Degraded,
                [ readinessTag ]
            )
        )
    |> ignore

    services

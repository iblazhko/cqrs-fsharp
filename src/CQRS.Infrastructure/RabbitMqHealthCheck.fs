namespace CQRS.Infrastructure

open System.Threading
open System.Threading.Tasks
open CQRS.Configuration.Infrastructure
open Microsoft.Extensions.Diagnostics.HealthChecks

type RabbitMqHealthCheck(connectionString: string) =
    interface IHealthCheck with
        member this.CheckHealthAsync(context: HealthCheckContext, cancellationToken: CancellationToken) =
            Task.FromResult(HealthCheckResult.Healthy($"{(nameof RabbitMqHealthCheck)}: ????"))

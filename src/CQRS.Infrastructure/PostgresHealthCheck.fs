namespace CQRS.Infrastructure

open System.Threading
open System.Threading.Tasks
open Microsoft.Extensions.Diagnostics.HealthChecks

type PostgresHealthCheck(connectionString: string) =
    interface IHealthCheck with
        member this.CheckHealthAsync(context: HealthCheckContext, cancellationToken: CancellationToken) =
            Task.FromResult(HealthCheckResult.Healthy($"{(nameof PostgresHealthCheck)}: ????"))

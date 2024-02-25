module CQRS.Infrastructure.ApplicationEnvironmentConfigurator

open System
open CQRS.Application
open Microsoft.Extensions.DependencyInjection

let configureServices (services: IServiceCollection) =

    services.AddSingleton<TimeProvider>(TimeProvider.System) |> ignore

    services.AddSingleton<IMoonPhaseService>(MoonPhaseServiceStub() :> IMoonPhaseService)
    |> ignore

    services

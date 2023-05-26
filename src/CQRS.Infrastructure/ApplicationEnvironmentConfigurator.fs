module CQRS.Infrastructure.ApplicationEnvironmentConfigurator

open CQRS.Application
open CQRS.Ports.Time
open Microsoft.Extensions.DependencyInjection

let configureServices (services: IServiceCollection) =

    services.AddSingleton<IClock>(SystemClock() :> IClock) |> ignore

    services.AddSingleton<IMoonPhaseService>(MoonPhaseServiceStub() :> IMoonPhaseService)
    |> ignore

    services

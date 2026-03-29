module CQRS.Infrastructure.ApplicationEnvironmentConfigurator

open System
open CQRS.Application
open CQRS.Application.CommandProcessingStatusRecording
open CQRS.Ports.EventStore
open Microsoft.Extensions.DependencyInjection

let configureServices (location: string) (services: IServiceCollection) =

    services.AddSingleton<TimeProvider>(TimeProvider.System) |> ignore

    services.AddSingleton<IMoonPhaseService>(MoonPhaseServiceStub() :> IMoonPhaseService)
    |> ignore

    services.AddSingleton<ICommandProcessingStatusRecordingService, CommandProcessingStatusRecordingService>()
    |> ignore

    services.AddSingleton<ApplicationEnvironment>(fun serviceProvider ->
        { Location = location
          Clock = serviceProvider.GetRequiredService<TimeProvider>()
          EventStore = serviceProvider.GetRequiredService<IEventStore>()
          MoonPhase = serviceProvider.GetRequiredService<IMoonPhaseService>()
          CommandProcessingStatusRecorder = serviceProvider.GetRequiredService<ICommandProcessingStatusRecordingService>() })
    |> ignore

    services

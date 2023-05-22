module CQRS.Infrastructure.LoggingConfigurator

open CQRS.Configuration.Infrastructure
open Microsoft.Extensions.Logging
open Serilog

let private setMinimumLevel (level: string) (loggerConfiguration: LoggerConfiguration) =
    match level with
    | "Verbose" -> loggerConfiguration.MinimumLevel.Verbose()
    | "Debug" -> loggerConfiguration.MinimumLevel.Debug()
    | "Information" -> loggerConfiguration.MinimumLevel.Information()
    | "Warning" -> loggerConfiguration.MinimumLevel.Warning()
    | "Error" -> loggerConfiguration.MinimumLevel.Error()
    | "Fatal" -> loggerConfiguration.MinimumLevel.Fatal()
    | _ -> loggerConfiguration

let configureLogging (settings: LoggingSettings) (builder: ILoggingBuilder) =
    Log.Logger <-
        (LoggerConfiguration() |> setMinimumLevel settings.Level)
            .WriteTo.Console()
            .CreateLogger()

    builder.ClearProviders() |> ignore
    builder.AddSerilog() |> ignore

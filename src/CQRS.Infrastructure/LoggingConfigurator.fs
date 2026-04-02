module CQRS.Infrastructure.LoggingConfigurator

open CQRS.Configuration.Infrastructure
open Microsoft.Extensions.Logging
open Serilog

let private setMinimumLevel (level: string) (loggerConfiguration: LoggerConfiguration) =
    match level.ToUpperInvariant() with
    | "VERBOSE" -> loggerConfiguration.MinimumLevel.Verbose()
    | "DEBUG" -> loggerConfiguration.MinimumLevel.Debug()
    | "INF" -> loggerConfiguration.MinimumLevel.Information()
    | "INFO" -> loggerConfiguration.MinimumLevel.Information()
    | "INFORMATION" -> loggerConfiguration.MinimumLevel.Information()
    | "WRN" -> loggerConfiguration.MinimumLevel.Warning()
    | "WARN" -> loggerConfiguration.MinimumLevel.Warning()
    | "WARNING" -> loggerConfiguration.MinimumLevel.Warning()
    | "ERR" -> loggerConfiguration.MinimumLevel.Error()
    | "ERROR" -> loggerConfiguration.MinimumLevel.Error()
    | "FATAL" -> loggerConfiguration.MinimumLevel.Fatal()
    | _ -> loggerConfiguration

let configureLogging (settings: LoggingSettings) (builder: ILoggingBuilder) =
    Log.Logger <-
        (LoggerConfiguration() |> setMinimumLevel settings.Level)
            .WriteTo.Console()
            .CreateLogger()

    builder.ClearProviders() |> ignore
    builder.AddSerilog() |> ignore

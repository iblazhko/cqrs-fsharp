namespace CQRS.Configuration

open System
open System.Text
open Microsoft.Extensions.Configuration
open CQRS.Configuration.Infrastructure
open CQRS.Configuration.SettingStringBuilder

type CqrsHostSettings() =
    member val ServiceUrl: string = String.Empty with get, set
    member val MartenDb: MartenDbSettings = MartenDbSettings.Empty with get, set
    member val MassTransit: MassTransitSettings = MassTransitSettings.Empty with get, set
    member val InfrastructureStartup: InfrastructureStartupSettings = InfrastructureStartupSettings.Empty with get, set
    member val Logging: LoggingSettings = LoggingSettings.Empty with get, set

    member this.getStartupInfo(hostName: string) =
        StringBuilder()
        |> appendSettingsTitle hostName
        |> appendSettingValue this.ServiceUrl (nameof this.ServiceUrl)
        |> appendSettingSection this.MartenDb (nameof this.MartenDb)
        |> appendSettingSection this.MassTransit (nameof this.MassTransit)
        |> appendSettingSection this.InfrastructureStartup (nameof this.InfrastructureStartup)
        |> appendSettingSection this.Logging (nameof this.Logging)
        |> fun b -> b.ToString()


module CqrsSettingsLoader =
    let getHostSettings (section: string) (argv: string[]) =
        let config =
            ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables("CQRS_")
                .AddCommandLine(argv)
                .Build()

        config.GetSection(section).Get<CqrsHostSettings>()

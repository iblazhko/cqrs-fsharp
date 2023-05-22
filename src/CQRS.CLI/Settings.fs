namespace CQRS.CLI.Settings

open Microsoft.Extensions.Configuration

type CqrsCliSettings = { ApiServiceUrl: string }

module CqrsSettingsLoader =
    let getCliSettings (section: string) (argv: string[]) =
        let config =
            ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables("CQRS_")
                .AddCommandLine(argv)
                .Build()

        config.GetSection(section).Get<CqrsCliSettings>()

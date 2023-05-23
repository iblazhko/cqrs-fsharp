module CQRS.CLI.Program

open CQRS.CLI.Settings
open McMaster.Extensions.CommandLineUtils
open Serilog

[<EntryPoint>]
let main (args: string[]) =
    let settings = CqrsSettingsLoader.getCliSettings "CqrsCli" args

    Log.Logger <- LoggerConfiguration().WriteTo.Console().CreateLogger()

    let cli = new CommandLineApplication()
    cli.Name <- "cqrs.cli"

    cli.Command(CreateInventoryCommand.name, (CreateInventoryCommand.configuration settings))
    |> ignore

    cli.Command(GetInventoryCommand.name, (GetInventoryCommand.configuration settings))
    |> ignore

    cli.HelpOption("-h|--help|-?") |> ignore
    cli.Execute(args)

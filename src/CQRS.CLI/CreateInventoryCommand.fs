module CQRS.CLI.CreateInventoryCommand

open CQRS.CLI.Settings
open CQRS.CLI.ApiClient
open McMaster.Extensions.CommandLineUtils

let name = "create"

let configuration (settings: CqrsCliSettings) (app: CommandLineApplication) =
    let inventoryName =
        app.Option<string>("-n|--name", "Inventory name", CommandOptionType.SingleValue)

    app.HelpOption("-? | -h | --help") |> ignore

    app.OnExecute(fun () ->
        (if inventoryName.HasValue() then
             settings.ApiServiceUrl |> createInventory inventoryName.ParsedValue
             0
         else
             app.Error.WriteLine("Inventory name must be provided")
             1))

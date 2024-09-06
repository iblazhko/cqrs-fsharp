module CQRS.CLI.DeactivateInventoryCommand

open CQRS.CLI.Settings
open CQRS.CLI.ApiClient
open McMaster.Extensions.CommandLineUtils

let name = "deactivate"

let configuration (settings: CqrsCliSettings) (app: CommandLineApplication) =
    let inventoryId =
        app.Option<string>("--id", "Inventory Id", CommandOptionType.SingleValue)

    app.HelpOption("-? | -h | --help") |> ignore

    app.OnExecute(fun () ->
        (if inventoryId.HasValue() then
             settings.ApiServiceUrl |> deactivateInventory inventoryId.ParsedValue
             0
         else
             app.Error.WriteLine("Inventory Id must be provided")
             1))

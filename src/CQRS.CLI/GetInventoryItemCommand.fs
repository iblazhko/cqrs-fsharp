module CQRS.CLI.GetInventoryItemCommand

open CQRS.CLI.Settings
open CQRS.CLI.ApiClient
open McMaster.Extensions.CommandLineUtils

let name = "get-inventory-item"

let configuration (settings: CqrsCliSettings) (app: CommandLineApplication) =
    let inventoryItemId =
        app.Option<string>("--id", "Inventory item Id", CommandOptionType.SingleValue)

    app.HelpOption("-? | -h | --help") |> ignore

    app.OnExecute(fun () ->
        (if inventoryItemId.HasValue() then
             settings.ApiServiceUrl |> getInventoryItem inventoryItemId.ParsedValue

             0
         else
             app.Error.WriteLine("Inventory item Id must be provided")
             1))

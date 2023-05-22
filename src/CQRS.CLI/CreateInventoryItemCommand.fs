module CQRS.CLI.CreateInventoryItemCommand

open CQRS.CLI.Settings
open CQRS.CLI.ApiClient
open McMaster.Extensions.CommandLineUtils

let name = "create-inventory-item"

let configuration (settings: CqrsCliSettings) (app: CommandLineApplication) =
    let inventoryItemName =
        app.Option<string>("-n|--name", "Inventory item name", CommandOptionType.SingleValue)

    app.HelpOption("-? | -h | --help") |> ignore

    app.OnExecute(fun () ->
        (if inventoryItemName.HasValue() then
             settings.ApiServiceUrl |> createInventoryItem inventoryItemName.ParsedValue

             0
         else
             app.Error.WriteLine("Inventory item name must be provided")
             1))

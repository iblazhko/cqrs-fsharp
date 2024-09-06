module CQRS.CLI.AddItemsToInventoryCommand

open CQRS.CLI.Settings
open CQRS.CLI.ApiClient
open McMaster.Extensions.CommandLineUtils

let name = "add-items"

let configuration (settings: CqrsCliSettings) (app: CommandLineApplication) =
    let inventoryId =
        app.Option<string>("--id", "Inventory Id", CommandOptionType.SingleValue)

    let count =
        app.Option<int>("-c|--count", "Items count", CommandOptionType.SingleValue)

    app.HelpOption("-? | -h | --help") |> ignore

    app.OnExecute(fun () ->
        (if inventoryId.HasValue() && count.HasValue() then
             settings.ApiServiceUrl
             |> addItemsToInventory inventoryId.ParsedValue count.ParsedValue
             0
         else
             app.Error.WriteLine("Inventory Id and items count must be provided")
             1))

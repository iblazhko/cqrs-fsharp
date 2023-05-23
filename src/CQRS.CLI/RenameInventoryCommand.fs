module CQRS.CLI.RenameInventoryCommand

open CQRS.CLI.Settings
open CQRS.CLI.ApiClient
open McMaster.Extensions.CommandLineUtils

let name = "rename"

let configuration (settings: CqrsCliSettings) (app: CommandLineApplication) =
    let inventoryId =
        app.Option<string>("--id", "Inventory Id", CommandOptionType.SingleValue)

    let inventoryName =
        app.Option<string>("-n|--name", "Inventory name", CommandOptionType.SingleValue)

    app.HelpOption("-? | -h | --help") |> ignore

    app.OnExecute(fun () ->
        (if inventoryId.HasValue() && inventoryName.HasValue() then
             settings.ApiServiceUrl
             |> renameInventory inventoryId.ParsedValue inventoryName.ParsedValue

             0
         else
             app.Error.WriteLine("Inventory Id and name must be provided")
             1))

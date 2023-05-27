module CQRS.CLI.GetInventoryCommand

open CQRS.CLI.Settings
open CQRS.CLI.ApiClient
open McMaster.Extensions.CommandLineUtils

let name = "get"

let configuration (settings: CqrsCliSettings) (app: CommandLineApplication) =
    let inventoryId =
        app.Option<string>("--id", "Inventory Id", CommandOptionType.SingleValue)

    app.HelpOption("-? | -h | --help") |> ignore

    app.OnExecute(fun () ->
        (if inventoryId.HasValue() then
             settings.ApiServiceUrl |> getInventory inventoryId.ParsedValue

             0
         else
             app.Error.WriteLine("Inventory Id must be provided")
             1))

module CQRS.Domain.Tests.DomainTestsSetup

open CQRS.Domain.Inventory
open CQRS.Domain.ValueTypes

let testInventoryName (name: string) : InventoryName =
    InventoryName.create (
        match name |> MediumString.create "InventoryName" with
        | Ok x -> x
        | Error _ -> failwith "Internal error"
    )

let testStockQuantityNumber (quantity: int) : PositiveInteger =
    match quantity |> PositiveInteger.create "StockQuantity" with
    | Ok x -> x
    | Error _ -> failwith "Internal error"

let testStockQuantity (quantity: int) : StockQuantity =
    StockQuantity.create (
        match quantity |> PositiveInteger.create "StockQuantity" with
        | Ok x -> x
        | Error _ -> failwith "Internal error"
    )

let inventoryId = InventoryId.newId ()
let inventoryName = testInventoryName "INV-123"

let newState: InventoryState =
    { InventoryId = inventoryId
      Name = testInventoryName "N/A"
      StockQuantity = StockQuantity.Empty
      IsNew = true
      IsActive = true }

let currentState: InventoryState =
    { InventoryId = inventoryId
      Name = inventoryName
      StockQuantity = testStockQuantity 5
      IsNew = false
      IsActive = true }

let currentStateWithNoStock: InventoryState =
    { InventoryId = inventoryId
      Name = inventoryName
      StockQuantity = StockQuantity.Empty
      IsNew = false
      IsActive = true }

let deactivatedState: InventoryState =
    { InventoryId = inventoryId
      Name = inventoryName
      StockQuantity = StockQuantity.Empty
      IsNew = false
      IsActive = false }

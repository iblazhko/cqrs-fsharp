module CQRS.Domain.Tests.DomainTestsSetup

open CQRS.Domain.Inventory
open CQRS.Domain.ValueTypes

let createTestInventoryName (name: string) : InventoryName =
    name
    |> MediumString.create "InventoryName"
    |> Result.defaultWith (fun _ -> failwith "Internal error")
    |> InventoryName.create

let createTestStockQuantityNumber (quantity: int) : PositiveInteger =
    quantity
    |> PositiveInteger.create "StockQuantity"
    |> Result.defaultWith (fun _ -> failwith "Internal error")

let createTestStockQuantity (quantity: int) : StockQuantity =
    quantity
    |> PositiveInteger.create "StockQuantity"
    |> Result.defaultWith (fun _ -> failwith "Internal error")
    |> StockQuantity.create

let inventoryId = InventoryId.newId ()
let inventoryName = createTestInventoryName "INV-123"

let newState: InventoryState =
    { InventoryId = inventoryId
      Name = createTestInventoryName "N/A"
      StockQuantity = StockQuantity.Empty
      IsNew = true
      IsActive = true }

let currentState: InventoryState =
    { InventoryId = inventoryId
      Name = inventoryName
      StockQuantity = createTestStockQuantity 5
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

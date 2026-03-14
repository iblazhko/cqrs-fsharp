module CQRS.Domain.Tests.InventoryEventsApplierTests

open Xunit
open FsUnit

open CQRS.Domain
open CQRS.Domain.Inventory
open CQRS.Domain.Tests.DomainTestsSetup

[<Fact>]
let ``Apply InventoryCreated event`` () =
    let evt =
        InventoryEvent.InventoryCreated
            { InventoryId = inventoryId
              Name = inventoryName
              IsActive = true }

    InventoryStateProjection.apply newState evt
    |> should
        equal
        { InventoryId = inventoryId
          Name = inventoryName
          StockQuantity = StockQuantity.Empty
          IsNew = false
          IsActive = true }

[<Fact>]
let ``Apply InventoryDeactivated event`` () =
    let evt =
        InventoryEvent.InventoryDeactivated
            { InventoryId = inventoryId
              Name = inventoryName }

    InventoryStateProjection.apply currentStateWithNoStock evt
    |> should
        equal
        { InventoryId = inventoryId
          Name = inventoryName
          StockQuantity = StockQuantity.Empty
          IsNew = false
          IsActive = false }

[<Fact>]
let ``Apply InventoryRenamed event`` () =
    let newName = createTestInventoryName "INV-123-UPDATED"

    let evt =
        InventoryEvent.InventoryRenamed
            { InventoryId = inventoryId
              OldName = inventoryName
              NewName = newName }

    InventoryStateProjection.apply currentState evt
    |> should
        equal
        { InventoryId = currentState.InventoryId
          Name = newName
          StockQuantity = currentState.StockQuantity
          IsNew = currentState.IsNew
          IsActive = currentState.IsActive }

[<Fact>]
let ``Apply ItemsAddedToInventory event`` () =
    let evt =
        InventoryEvent.ItemsAddedToInventory
            { InventoryId = inventoryId
              Name = inventoryName
              OldStockQuantity = createTestStockQuantity 1
              NewStockQuantity = createTestStockQuantity 2
              AddedCount = createTestStockQuantityNumber 1 }

    InventoryStateProjection.apply currentState evt
    |> should
        equal
        { InventoryId = currentState.InventoryId
          Name = currentState.Name
          StockQuantity = createTestStockQuantity 2
          IsNew = currentState.IsNew
          IsActive = currentState.IsActive }

[<Fact>]
let ``Apply ItemsRemovedFromInventory event`` () =
    let evt =
        InventoryEvent.ItemsRemovedFromInventory
            { InventoryId = inventoryId
              Name = inventoryName
              OldStockQuantity = createTestStockQuantity 2
              NewStockQuantity = createTestStockQuantity 1
              RemovedCount = createTestStockQuantityNumber 1 }

    InventoryStateProjection.apply currentState evt
    |> should
        equal
        { InventoryId = currentState.InventoryId
          Name = currentState.Name
          StockQuantity = createTestStockQuantity 1
          IsNew = currentState.IsNew
          IsActive = currentState.IsActive }

[<Fact>]
let ``Apply ItemInStock event`` () =
    let evt =
        InventoryEvent.ItemInStock
            { InventoryId = inventoryId
              Name = inventoryName
              StockQuantity = createTestStockQuantity 3 }

    InventoryStateProjection.apply currentState evt
    |> should
        equal
        { InventoryId = currentState.InventoryId
          Name = currentState.Name
          StockQuantity = currentState.StockQuantity
          IsNew = currentState.IsNew
          IsActive = currentState.IsActive }

[<Fact>]
let ``Apply ItemWentOutOfStock event`` () =
    let evt =
        InventoryEvent.ItemWentOutOfStock
            { InventoryId = inventoryId
              Name = inventoryName }

    InventoryStateProjection.apply currentState evt
    |> should
        equal
        { InventoryId = currentState.InventoryId
          Name = currentState.Name
          StockQuantity = currentState.StockQuantity
          IsNew = currentState.IsNew
          IsActive = currentState.IsActive }

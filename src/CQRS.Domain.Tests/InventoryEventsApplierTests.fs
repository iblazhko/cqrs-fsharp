module CQRS.Domain.Tests.InventoryEventsApplierTests

open Xunit
open FsUnit

open CQRS.Domain
open CQRS.Domain.Inventory
open CQRS.Domain.Tests.DomainTestsSetup

[<Fact>]
let ``Apply InventoryCreated event`` () =
    let evt =
        InventoryCreated
            { InventoryId = inventoryId
              Name = inventoryName
              IsActive = true }

    InventoryStateProjection.apply newState evt
    |> should
        equal
        (Active
            { InventoryId = inventoryId
              Name = inventoryName
              StockQuantity = Empty })

[<Fact>]
let ``Apply InventoryDeactivated event`` () =
    let evt =
        InventoryDeactivated
            { InventoryId = inventoryId
              Name = inventoryName }

    InventoryStateProjection.apply currentStateWithNoStock evt
    |> should
        equal
        (Inactive
            { InventoryId = inventoryId
              Name = inventoryName
              StockQuantity = Empty })

[<Fact>]
let ``Apply InventoryRenamed event`` () =
    let newName = createTestInventoryName "INV-123-UPDATED"

    let evt =
        InventoryRenamed
            { InventoryId = inventoryId
              OldName = inventoryName
              NewName = newName }

    InventoryStateProjection.apply currentState evt
    |> should
        equal
        (Active
            { InventoryId = inventoryId
              Name = newName
              StockQuantity = createTestStockQuantity 5 })

[<Fact>]
let ``Apply ItemsAddedToInventory event`` () =
    let evt =
        ItemsAddedToInventory
            { InventoryId = inventoryId
              Name = inventoryName
              OldStockQuantity = createTestStockQuantity 1
              NewStockQuantity = createTestStockQuantity 2
              AddedCount = createTestStockQuantityNumber 1 }

    InventoryStateProjection.apply currentState evt
    |> should
        equal
        (Active
            { InventoryId = inventoryId
              Name = inventoryName
              StockQuantity = createTestStockQuantity 2 })

[<Fact>]
let ``Apply ItemsRemovedFromInventory event`` () =
    let evt =
        ItemsRemovedFromInventory
            { InventoryId = inventoryId
              Name = inventoryName
              OldStockQuantity = createTestStockQuantity 2
              NewStockQuantity = createTestStockQuantity 1
              RemovedCount = createTestStockQuantityNumber 1 }

    InventoryStateProjection.apply currentState evt
    |> should
        equal
        (Active
            { InventoryId = inventoryId
              Name = inventoryName
              StockQuantity = createTestStockQuantity 1 })

[<Fact>]
let ``Apply ItemInStock event`` () =
    let evt =
        ItemInStock
            { InventoryId = inventoryId
              Name = inventoryName
              StockQuantity = createTestStockQuantity 3 }

    InventoryStateProjection.apply currentState evt
    |> should equal currentState

[<Fact>]
let ``Apply ItemWentOutOfStock event`` () =
    let evt =
        ItemWentOutOfStock
            { InventoryId = inventoryId
              Name = inventoryName }

    InventoryStateProjection.apply currentState evt
    |> should equal currentState

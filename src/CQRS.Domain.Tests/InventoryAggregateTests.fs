module CQRS.Domain.Tests.InventoryAggregateTests

open Xunit

open CQRS.Domain
open CQRS.Domain.Inventory
open CQRS.Domain.Tests.DomainTestsSetup
open CQRS.Domain.Tests.AggregateAssertions

[<Fact>]
let ``Inventory can be created`` () =
    InventoryAggregate.create
        newState
        { InventoryId = inventoryId
          Name = inventoryName }
    |> assertAggregateSuccess (
        seq {
            InventoryCreated
                { InventoryId = inventoryId
                  Name = inventoryName
                  IsActive = true }
        }
    )

[<Fact>]
let ``Inventory cannot be created if it already exists`` () =
    InventoryAggregate.create
        currentState
        { InventoryId = inventoryId
          Name = inventoryName }
    |> assertAggregateFailure (AlreadyExists inventoryId)

    InventoryAggregate.create
        deactivatedState
        { InventoryId = inventoryId
          Name = inventoryName }
    |> assertAggregateFailure (AlreadyExists inventoryId)

[<Fact>]
let ``Inventory can be renamed`` () =
    let newName = createTestInventoryName "INV-123-UPDATED"

    InventoryAggregate.rename
        currentState
        { InventoryId = inventoryId
          NewName = newName }
    |> assertAggregateSuccess (
        seq {
            InventoryRenamed
                { InventoryId = inventoryId
                  OldName = inventoryName
                  NewName = newName }
        }
    )

[<Fact>]
let ``Inventory cannot be renamed if it does not exist`` () =
    InventoryAggregate.rename
        newState
        { InventoryId = inventoryId
          NewName = createTestInventoryName "INV-123-UPDATED" }
    |> assertAggregateFailure (DoesNotExist inventoryId)

[<Fact>]
let ``Inventory cannot be renamed if it is deactivated`` () =
    InventoryAggregate.rename
        deactivatedState
        { InventoryId = inventoryId
          NewName = createTestInventoryName "INV-123-UPDATED" }
    |> assertAggregateFailure (Deactivated inventoryId)

[<Fact>]
let ``Inventory renaming using same name does not produce new events`` () =
    InventoryAggregate.rename
        currentState
        { InventoryId = inventoryId
          NewName = inventoryName }
    |> assertAggregateSuccess Seq.empty

[<Fact>]
let ``Items can be added to Inventory`` () =
    let countToAdd = createTestStockQuantityNumber 1
    let currentStock = createTestStockQuantity 5

    InventoryAggregate.addItems
        currentState
        { InventoryId = inventoryId
          Count = countToAdd }
    |> assertAggregateSuccess (
        seq {
            ItemsAddedToInventory
                { InventoryId = inventoryId
                  Name = inventoryName
                  AddedCount = countToAdd
                  OldStockQuantity = currentStock
                  NewStockQuantity = StockQuantity.add currentStock countToAdd }
        }
    )

[<Fact>]
let ``Adding items to empty Inventory produces InStock event`` () =
    let countToAdd = createTestStockQuantityNumber 1

    InventoryAggregate.addItems
        currentStateWithNoStock
        { InventoryId = inventoryId
          Count = countToAdd }
    |> assertAggregateSuccess (
        seq {
            ItemsAddedToInventory
                { InventoryId = inventoryId
                  Name = inventoryName
                  AddedCount = countToAdd
                  OldStockQuantity = Empty
                  NewStockQuantity = InventoryCount countToAdd }

            ItemInStock
                { InventoryId = inventoryId
                  Name = inventoryName
                  StockQuantity = InventoryCount countToAdd }
        }
    )

[<Fact>]
let ``Items cannot be added to deactivated Inventory`` () =
    InventoryAggregate.addItems
        deactivatedState
        { InventoryId = inventoryId
          Count = createTestStockQuantityNumber 1 }
    |> assertAggregateFailure (Deactivated inventoryId)

[<Fact>]
let ``Items can be removed from Inventory`` () =
    let countToRemove = createTestStockQuantityNumber 1

    InventoryAggregate.removeItems
        currentState
        { InventoryId = inventoryId
          Count = countToRemove }
    |> assertAggregateSuccess (
        seq {
            ItemsRemovedFromInventory
                { InventoryId = inventoryId
                  Name = inventoryName
                  RemovedCount = countToRemove
                  OldStockQuantity = createTestStockQuantity 5
                  NewStockQuantity = createTestStockQuantity 4 }
        }
    )

[<Fact>]
let ``Removing all items from Inventory produces OutOfStock event`` () =
    let stockQuantity = createTestStockQuantityNumber 5

    InventoryAggregate.removeItems
        currentState
        { InventoryId = inventoryId
          Count = stockQuantity }
    |> assertAggregateSuccess (
        seq {
            ItemsRemovedFromInventory
                { InventoryId = inventoryId
                  Name = inventoryName
                  RemovedCount = stockQuantity
                  OldStockQuantity = InventoryCount stockQuantity
                  NewStockQuantity = Empty }

            ItemWentOutOfStock
                { InventoryId = inventoryId
                  Name = inventoryName }
        }
    )

[<Fact>]
let ``Items cannot be removed if not enough items available in stock`` () =
    InventoryAggregate.removeItems
        currentStateWithNoStock
        { InventoryId = inventoryId
          Count = createTestStockQuantityNumber 1 }
    |> assertAggregateFailure (CannotRequestMoreThanHaveInStock inventoryId)

    InventoryAggregate.removeItems
        currentState
        { InventoryId = inventoryId
          Count = createTestStockQuantityNumber 6 }
    |> assertAggregateFailure (CannotRequestMoreThanHaveInStock inventoryId)

[<Fact>]
let ``Inventory can be deactivated if no items available in stock`` () =
    InventoryAggregate.deactivate currentStateWithNoStock Moon.NewMoon { InventoryId = inventoryId }
    |> assertAggregateSuccess (
        seq {
            InventoryDeactivated
                { InventoryId = inventoryId
                  Name = inventoryName }
        }
    )

[<Fact>]
let ``Inventory cannot be deactivated if there are items available in stock`` () =
    InventoryAggregate.deactivate currentState Moon.NewMoon { InventoryId = inventoryId }
    |> assertAggregateFailure (CannotDeactivateNonEmpty inventoryId)

[<Fact>]
let ``Inventory cannot be deactivated when the moon is in full phase`` () =
    InventoryAggregate.deactivate currentStateWithNoStock Moon.FullMoon { InventoryId = inventoryId }
    |> assertAggregateFailure (CannotDeactivateWhenMoonIsFull inventoryId)

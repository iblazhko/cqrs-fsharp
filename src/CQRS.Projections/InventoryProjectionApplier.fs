module CQRS.Projections.InventoryProjectionApplier

open CQRS.Domain.Inventory
open CQRS.Mapping

let applyInventoryCreated (evt: InventoryCreated) (vm: InventoryViewModel) =
    vm.InventoryId <- evt.InventoryId |> InventoryIdMapper.fromDomain
    vm.Name <- evt.Name |> InventoryNameMapper.fromDomain
    vm.IsActive <- evt.IsActive
    vm

let applyInventoryRenamed (evt: InventoryRenamed) (vm: InventoryViewModel) =
    vm.Name <- evt.NewName |> InventoryNameMapper.fromDomain
    vm

let applyItemsAddedToInventory (evt: ItemsAddedToInventory) (vm: InventoryViewModel) =
    vm.StockQuantity <- evt.NewStockQuantity |> StockQuantityMapper.fromDomain
    vm

let applyItemsRemovedFromInventory (evt: ItemsRemovedFromInventory) (vm: InventoryViewModel) =
    vm.StockQuantity <- evt.NewStockQuantity |> StockQuantityMapper.fromDomain
    vm

let applyInventoryDeactivated (_: InventoryDeactivated) (vm: InventoryViewModel) =
    vm.IsActive <- false
    vm

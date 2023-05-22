module CQRS.Projections.InventoryItemProjectionApplier

open CQRS.Domain.Inventory

let applyInventoryItemCreated (evt: InventoryItemCreated) (vm: InventoryItemViewModel) = vm

let applyInventoryItemRenamed (evt: InventoryItemRenamed) (vm: InventoryItemViewModel) = vm

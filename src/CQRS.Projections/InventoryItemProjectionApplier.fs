module CQRS.Projections.InventoryItemProjectionApplier

open CQRS.Domain.Inventory
open CQRS.Mapping

let applyInventoryItemCreated (evt: InventoryItemCreated) (vm: InventoryItemViewModel) =
    vm.InventoryItemId <- evt.InventoryItemId |> InventoryItemIdMapper.fromDomain
    vm.Name <- evt.Name |> InventoryItemNameMapper.fromDomain
    vm

let applyInventoryItemRenamed (evt: InventoryItemRenamed) (vm: InventoryItemViewModel) =
    vm.Name <- evt.NewName |> InventoryItemNameMapper.fromDomain
    vm

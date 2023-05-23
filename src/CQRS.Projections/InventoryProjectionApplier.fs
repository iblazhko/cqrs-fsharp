module CQRS.Projections.InventoryProjectionApplier

open CQRS.Domain.Inventory
open CQRS.Mapping

let applyInventoryCreated (evt: InventoryCreated) (vm: InventoryViewModel) =
    vm.InventoryId <- evt.InventoryId |> InventoryIdMapper.fromDomain
    vm.Name <- evt.Name |> InventoryNameMapper.fromDomain
    vm

let applyInventoryRenamed (evt: InventoryRenamed) (vm: InventoryViewModel) =
    vm.Name <- evt.NewName |> InventoryNameMapper.fromDomain
    vm

module CQRS.Mapping.DeactivateInventoryMapper

open CQRS.Domain
open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.DtoMapper
open FsToolkit.ErrorHandling

let fromDomain (domain: Inventory.DeactivateInventory) =
    let dto = DeactivateInventoryCommand()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.fromDomain
    dto

let toDomain (dto: DeactivateInventoryCommand) =
    result {
        let! nonNullDto = dto |> ensureNotNull
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.toDomain "InventoryId"
        return { DeactivateInventory.InventoryId = inventoryId }
    }

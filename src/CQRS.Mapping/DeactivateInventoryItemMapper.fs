module CQRS.Mapping.DeactivateInventoryItemMapper

open CQRS.Domain
open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.DtoMapper
open FsToolkit.ErrorHandling

let fromDomain (domain: Inventory.DeactivateInventoryItem) =
    let dto = DeactivateInventoryItemCommand()
    dto.InventoryItemId <- domain.InventoryItemId |> InventoryItemIdMapper.fromDomain
    dto

let toDomain (dto: DeactivateInventoryItemCommand) =
    result {
        let! nonNullDto = dto |> ensureNotNull
        let! inventoryItemId = nonNullDto.InventoryItemId |> InventoryItemIdMapper.toDomain "InventoryItemId"
        return { DeactivateInventoryItem.InventoryItemId = inventoryItemId }
    }

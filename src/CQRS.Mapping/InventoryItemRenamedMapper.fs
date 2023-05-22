module CQRS.Mapping.InventoryItemRenamedMapper

open CQRS.Domain.Inventory
open CQRS.Domain
open CQRS.DTO.V1
open CQRS.Mapping.DtoMapper
open FsToolkit.ErrorHandling

let fromDomain (domain: Inventory.InventoryItemRenamed) =
    let dto = InventoryItemRenamedEvent()
    dto.InventoryItemId <- domain.InventoryItemId |> InventoryItemIdMapper.fromDomain
    dto.OldName <- domain.OldName |> InventoryItemNameMapper.fromDomain
    dto.NewName <- domain.NewName |> InventoryItemNameMapper.fromDomain
    dto

let toDomain (dto: InventoryItemRenamedEvent) =
    result {
        let! nonNullDto = dto |> ensureNotNull
        let! inventoryItemId = nonNullDto.InventoryItemId |> InventoryItemIdMapper.toDomain "InventoryItemId"
        let! oldName = nonNullDto.OldName |> InventoryItemNameMapper.toDomain "OldName"
        let! newName = nonNullDto.NewName |> InventoryItemNameMapper.toDomain "NewName"

        return
            { InventoryItemRenamed.InventoryItemId = inventoryItemId
              OldName = oldName
              NewName = newName }
    }

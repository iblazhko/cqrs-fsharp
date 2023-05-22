module CQRS.Mapping.RenameInventoryItemMapper

open CQRS.Domain
open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.DtoMapper
open FsToolkit.ErrorHandling

let fromDomain (domain: Inventory.RenameInventoryItem) =
    let dto = RenameInventoryItemCommand()
    dto.InventoryItemId <- domain.InventoryItemId |> InventoryItemIdMapper.fromDomain
    dto.NewName <- domain.NewName |> InventoryItemNameMapper.fromDomain
    dto

let toDomain (dto: RenameInventoryItemCommand) =
    result {
        let! nonNullDto = dto |> ensureNotNull
        let! inventoryItemId = nonNullDto.InventoryItemId |> InventoryItemIdMapper.toDomain "InventoryItemId"
        let! newName = nonNullDto.NewName |> InventoryItemNameMapper.toDomain "NewName"

        return
            { RenameInventoryItem.InventoryItemId = inventoryItemId
              NewName = newName }
    }

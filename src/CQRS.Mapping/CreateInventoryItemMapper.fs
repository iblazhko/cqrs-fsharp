module CQRS.Mapping.CreateInventoryItemMapper

open CQRS.Domain
open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.DtoMapper
open FsToolkit.ErrorHandling

let fromDomain (domain: Inventory.CreateInventoryItem) =
    let dto = CreateInventoryItemCommand()
    dto.InventoryItemId <- domain.InventoryItemId |> InventoryItemIdMapper.fromDomain
    dto.Name <- domain.Name |> InventoryItemNameMapper.fromDomain
    dto

let toDomain (dto: CreateInventoryItemCommand) =
    result {
        let! nonNullDto = dto |> ensureNotNull
        let! inventoryItemId = nonNullDto.InventoryItemId |> InventoryItemIdMapper.toDomain "InventoryItemId"
        let! inventoryItemName = nonNullDto.Name |> InventoryItemNameMapper.toDomain "Name"

        return
            { CreateInventoryItem.InventoryItemId = inventoryItemId
              Name = inventoryItemName }
    }

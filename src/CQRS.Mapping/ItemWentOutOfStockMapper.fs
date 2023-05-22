module CQRS.Mapping.ItemWentOutOfStockMapper

open CQRS.Domain.Inventory
open CQRS.Domain
open CQRS.DTO.V1
open CQRS.Mapping.DtoMapper
open FsToolkit.ErrorHandling

let fromDomain (domain: Inventory.ItemWentOutOfStock) =
    let dto = ItemWentOutOfStockEvent()
    dto.InventoryItemId <- domain.InventoryItemId |> InventoryItemIdMapper.fromDomain
    dto.Name <- domain.Name |> InventoryItemNameMapper.fromDomain
    dto

let toDomain (dto: ItemWentOutOfStockEvent) =
    result {
        let! nonNullDto = dto |> ensureNotNull
        let! inventoryItemId = nonNullDto.InventoryItemId |> InventoryItemIdMapper.toDomain "InventoryItemId"
        let! name = nonNullDto.Name |> InventoryItemNameMapper.toDomain "Name"

        return
            { ItemWentOutOfStock.InventoryItemId = inventoryItemId
              Name = name }
    }

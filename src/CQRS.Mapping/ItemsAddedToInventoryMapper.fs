module CQRS.Mapping.ItemsAddedToInventoryMapper

open CQRS.Domain.Inventory
open CQRS.Domain
open CQRS.DTO.V1
open CQRS.Mapping.DtoMapper
open FsToolkit.ErrorHandling

let fromDomain (domain: Inventory.ItemsAddedToInventory) =
    let dto = ItemsAddedToInventoryEvent()
    dto.InventoryItemId <- domain.InventoryItemId |> InventoryItemIdMapper.fromDomain
    dto.Name <- domain.Name |> InventoryItemNameMapper.fromDomain
    dto.AddedCount <- domain.AddedCount |> CountMapper.fromDomain
    dto.OldStockQuantity <- domain.OldStockQuantity |> StockQuantityMapper.fromDomain
    dto.NewStockQuantity <- domain.NewStockQuantity |> StockQuantityMapper.fromDomain
    dto

let toDomain (dto: ItemsAddedToInventoryEvent) =
    result {
        let! nonNullDto = dto |> ensureNotNull
        let! inventoryItemId = nonNullDto.InventoryItemId |> InventoryItemIdMapper.toDomain "InventoryItemId"
        let! name = nonNullDto.Name |> InventoryItemNameMapper.toDomain "Name"
        let! addedCount = nonNullDto.AddedCount |> CountMapper.toDomain "AddedCount"
        let! oldStockQuantity = nonNullDto.OldStockQuantity |> StockQuantityMapper.toDomain "OldStockQuantity"
        let! newStockQuantity = nonNullDto.NewStockQuantity |> StockQuantityMapper.toDomain "NewStockQuantity"

        return
            { ItemsAddedToInventory.InventoryItemId = inventoryItemId
              Name = name
              AddedCount = addedCount
              OldStockQuantity = oldStockQuantity
              NewStockQuantity = newStockQuantity }
    }

module CQRS.Mapping.ItemsRemovedFromInventoryMapper

open CQRS.Domain.Inventory
open CQRS.Domain
open CQRS.DTO.V1
open CQRS.Mapping.DtoMapper
open FsToolkit.ErrorHandling

let fromDomain (domain: Inventory.ItemsRemovedFromInventory) =
    let dto = ItemsRemovedFromInventoryEvent()
    dto.InventoryItemId <- domain.InventoryItemId |> InventoryItemIdMapper.fromDomain
    dto.Name <- domain.Name |> InventoryItemNameMapper.fromDomain
    dto.RemovedCount <- domain.RemovedCount |> CountMapper.fromDomain
    dto.OldStockQuantity <- domain.OldStockQuantity |> StockQuantityMapper.fromDomain
    dto.NewStockQuantity <- domain.NewStockQuantity |> StockQuantityMapper.fromDomain
    dto

let toDomain (dto: ItemsRemovedFromInventoryEvent) =
    result {
        let! nonNullDto = dto |> ensureNotNull
        let! inventoryItemId = nonNullDto.InventoryItemId |> InventoryItemIdMapper.toDomain "InventoryItemId"
        let! name = nonNullDto.Name |> InventoryItemNameMapper.toDomain "Name"
        let! removedCount = nonNullDto.RemovedCount |> CountMapper.toDomain "RemovedCount"
        let! oldStockQuantity = nonNullDto.OldStockQuantity |> StockQuantityMapper.toDomain "OldStockQuantity"
        let! newStockQuantity = nonNullDto.NewStockQuantity |> StockQuantityMapper.toDomain "NewStockQuantity"

        return
            { ItemsRemovedFromInventory.InventoryItemId = inventoryItemId
              Name = name
              RemovedCount = removedCount
              OldStockQuantity = oldStockQuantity
              NewStockQuantity = newStockQuantity }
    }

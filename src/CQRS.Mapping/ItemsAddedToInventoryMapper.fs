module CQRS.Mapping.ItemsAddedToInventoryMapper

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.DtoMapper
open FsToolkit.ErrorHandling

let fromDomain (domain: ItemsAddedToInventory) =
    let dto = ItemsAddedToInventoryEvent()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.fromDomain
    dto.Name <- domain.Name |> InventoryNameMapper.fromDomain
    dto.AddedCount <- domain.AddedCount |> CountMapper.fromDomain
    dto.OldStockQuantity <- domain.OldStockQuantity |> StockQuantityMapper.fromDomain
    dto.NewStockQuantity <- domain.NewStockQuantity |> StockQuantityMapper.fromDomain
    dto

let toDomain (dto: ItemsAddedToInventoryEvent) =
    result {
        let! nonNullDto = dto |> ensureNotNull
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.toDomain "InventoryId"
        let! name = nonNullDto.Name |> InventoryNameMapper.toDomain "Name"
        let! addedCount = nonNullDto.AddedCount |> CountMapper.toDomain "AddedCount"
        let! oldStockQuantity = nonNullDto.OldStockQuantity |> StockQuantityMapper.toDomain "OldStockQuantity"
        let! newStockQuantity = nonNullDto.NewStockQuantity |> StockQuantityMapper.toDomain "NewStockQuantity"

        return
            { ItemsAddedToInventory.InventoryId = inventoryId
              Name = name
              AddedCount = addedCount
              OldStockQuantity = oldStockQuantity
              NewStockQuantity = newStockQuantity }
    }

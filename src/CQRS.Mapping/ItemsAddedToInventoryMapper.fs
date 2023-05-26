module CQRS.Mapping.ItemsAddedToInventoryMapper

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.NullableDtoMapper
open FsToolkit.ErrorHandling

let toDTO (domain: ItemsAddedToInventory) =
    let dto = ItemsAddedToInventoryEvent()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.toDTO
    dto.Name <- domain.Name |> InventoryNameMapper.toDTO
    dto.AddedCount <- domain.AddedCount |> CountMapper.toDTO
    dto.OldStockQuantity <- domain.OldStockQuantity |> StockQuantityMapper.toDTO
    dto.NewStockQuantity <- domain.NewStockQuantity |> StockQuantityMapper.toDTO
    dto

let ofDTO (dto: ItemsAddedToInventoryEvent) =
    result {
        let! nonNullDto = dto |> ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.ofDTO "InventoryId"
        let! name = nonNullDto.Name |> InventoryNameMapper.ofDTO "Name"
        let! addedCount = nonNullDto.AddedCount |> CountMapper.ofDTO "AddedCount"
        let! oldStockQuantity = nonNullDto.OldStockQuantity |> StockQuantityMapper.ofDTO "OldStockQuantity"
        let! newStockQuantity = nonNullDto.NewStockQuantity |> StockQuantityMapper.ofDTO "NewStockQuantity"

        return
            { ItemsAddedToInventory.InventoryId = inventoryId
              Name = name
              AddedCount = addedCount
              OldStockQuantity = oldStockQuantity
              NewStockQuantity = newStockQuantity }
    }

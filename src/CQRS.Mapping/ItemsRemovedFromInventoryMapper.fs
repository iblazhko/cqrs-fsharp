module CQRS.Mapping.ItemsRemovedFromInventoryMapper

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.NullableDtoMapper
open FsToolkit.ErrorHandling

let toDTO (domain: ItemsRemovedFromInventory) =
    let dto = ItemsRemovedFromInventoryEvent()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.toDTO
    dto.Name <- domain.Name |> InventoryNameMapper.toDTO
    dto.RemovedCount <- domain.RemovedCount |> CountMapper.toDTO
    dto.OldStockQuantity <- domain.OldStockQuantity |> StockQuantityMapper.toDTO
    dto.NewStockQuantity <- domain.NewStockQuantity |> StockQuantityMapper.toDTO
    dto

let ofDTO (dto: ItemsRemovedFromInventoryEvent) =
    result {
        let! nonNullDto = dto |> ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.ofDTO "InventoryId"
        let! name = nonNullDto.Name |> InventoryNameMapper.ofDTO "Name"
        let! removedCount = nonNullDto.RemovedCount |> CountMapper.ofDTO "RemovedCount"
        let! oldStockQuantity = nonNullDto.OldStockQuantity |> StockQuantityMapper.ofDTO "OldStockQuantity"
        let! newStockQuantity = nonNullDto.NewStockQuantity |> StockQuantityMapper.ofDTO "NewStockQuantity"

        return
            { ItemsRemovedFromInventory.InventoryId = inventoryId
              Name = name
              RemovedCount = removedCount
              OldStockQuantity = oldStockQuantity
              NewStockQuantity = newStockQuantity }
    }

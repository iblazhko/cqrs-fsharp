module CQRS.Mapping.ItemsRemovedFromInventoryMapper

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.DtoMapper
open FsToolkit.ErrorHandling

let fromDomain (domain: ItemsRemovedFromInventory) =
    let dto = ItemsRemovedFromInventoryEvent()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.fromDomain
    dto.Name <- domain.Name |> InventoryNameMapper.fromDomain
    dto.RemovedCount <- domain.RemovedCount |> CountMapper.fromDomain
    dto.OldStockQuantity <- domain.OldStockQuantity |> StockQuantityMapper.fromDomain
    dto.NewStockQuantity <- domain.NewStockQuantity |> StockQuantityMapper.fromDomain
    dto

let toDomain (dto: ItemsRemovedFromInventoryEvent) =
    result {
        let! nonNullDto = dto |> ensureNotNull
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.toDomain "InventoryId"
        let! name = nonNullDto.Name |> InventoryNameMapper.toDomain "Name"
        let! removedCount = nonNullDto.RemovedCount |> CountMapper.toDomain "RemovedCount"
        let! oldStockQuantity = nonNullDto.OldStockQuantity |> StockQuantityMapper.toDomain "OldStockQuantity"
        let! newStockQuantity = nonNullDto.NewStockQuantity |> StockQuantityMapper.toDomain "NewStockQuantity"

        return
            { ItemsRemovedFromInventory.InventoryId = inventoryId
              Name = name
              RemovedCount = removedCount
              OldStockQuantity = oldStockQuantity
              NewStockQuantity = newStockQuantity }
    }

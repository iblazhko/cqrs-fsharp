module CQRS.Mapping.ItemsRemovedFromInventory'

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open FsToolkit.ErrorHandling

let toDTO (domain: ItemsRemovedFromInventory) =
    let dto = ItemsRemovedFromInventoryEvent()
    dto.InventoryId <- domain.InventoryId |> InventoryId'.toDTO
    dto.Name <- domain.Name |> InventoryName'.toDTO
    dto.RemovedCount <- domain.RemovedCount |> Count'.toDTO
    dto.OldStockQuantity <- domain.OldStockQuantity |> StockQuantity'.toDTO
    dto.NewStockQuantity <- domain.NewStockQuantity |> StockQuantity'.toDTO
    dto

let ofDTO (dto: ItemsRemovedFromInventoryEvent) =
    result {
        let! nonNullDto = dto |> NullableDto'.ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryId'.ofDTO "InventoryId"
        let! name = nonNullDto.Name |> InventoryName'.ofDTO "Name"
        let! removedCount = nonNullDto.RemovedCount |> Count'.ofDTO "RemovedCount"
        let! oldStockQuantity = nonNullDto.OldStockQuantity |> StockQuantity'.ofDTO "OldStockQuantity"
        let! newStockQuantity = nonNullDto.NewStockQuantity |> StockQuantity'.ofDTO "NewStockQuantity"

        return
            { ItemsRemovedFromInventory.InventoryId = inventoryId
              Name = name
              RemovedCount = removedCount
              OldStockQuantity = oldStockQuantity
              NewStockQuantity = newStockQuantity }
    }

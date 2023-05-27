module CQRS.Mapping.ItemsAddedToInventory'

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open FsToolkit.ErrorHandling

let toDTO (domain: ItemsAddedToInventory) =
    let dto = ItemsAddedToInventoryEvent()
    dto.InventoryId <- domain.InventoryId |> InventoryId'.toDTO
    dto.Name <- domain.Name |> InventoryName'.toDTO
    dto.AddedCount <- domain.AddedCount |> Count'.toDTO
    dto.OldStockQuantity <- domain.OldStockQuantity |> StockQuantity'.toDTO
    dto.NewStockQuantity <- domain.NewStockQuantity |> StockQuantity'.toDTO
    dto

let ofDTO (dto: ItemsAddedToInventoryEvent) =
    result {
        let! nonNullDto = dto |> NullableDto'.ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryId'.ofDTO "InventoryId"
        let! name = nonNullDto.Name |> InventoryName'.ofDTO "Name"
        let! addedCount = nonNullDto.AddedCount |> Count'.ofDTO "AddedCount"
        let! oldStockQuantity = nonNullDto.OldStockQuantity |> StockQuantity'.ofDTO "OldStockQuantity"
        let! newStockQuantity = nonNullDto.NewStockQuantity |> StockQuantity'.ofDTO "NewStockQuantity"

        return
            { ItemsAddedToInventory.InventoryId = inventoryId
              Name = name
              AddedCount = addedCount
              OldStockQuantity = oldStockQuantity
              NewStockQuantity = newStockQuantity }
    }

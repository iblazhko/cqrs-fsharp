module CQRS.Mapping.ItemInStock'

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open FsToolkit.ErrorHandling

let toDTO (domain: ItemInStock) =
    let dto = ItemInStockEvent()
    dto.InventoryId <- domain.InventoryId |> InventoryId'.toDTO
    dto.Name <- domain.Name |> InventoryName'.toDTO
    dto.StockQuantity <- domain.StockQuantity |> StockQuantity'.toDTO
    dto

let ofDTO (dto: ItemInStockEvent) =
    result {
        let! nonNullDto = dto |> NullableDto'.ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryId'.ofDTO "InventoryId"
        let! name = nonNullDto.Name |> InventoryName'.ofDTO "Name"
        let! stockQuantity = nonNullDto.StockQuantity |> StockQuantity'.ofDTO "StockQuantity"

        return
            { ItemInStock.InventoryId = inventoryId
              Name = name
              StockQuantity = stockQuantity }
    }

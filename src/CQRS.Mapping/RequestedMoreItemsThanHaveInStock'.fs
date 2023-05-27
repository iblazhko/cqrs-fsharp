module CQRS.Mapping.RequestedMoreItemsThanHaveInStock'

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open FsToolkit.ErrorHandling

let toDTO (domain: RequestedMoreItemsThanHaveInStock) =
    let dto = RequestedMoreItemsThanHaveInStockEvent()
    dto.InventoryId <- domain.InventoryId |> InventoryId'.toDTO
    dto.Name <- domain.Name |> InventoryName'.toDTO
    dto.StockQuantity <- domain.StockQuantity |> StockQuantity'.toDTO
    dto.RequestedCount <- domain.RequestedCount |> Count'.toDTO
    dto

let ofDTO (dto: RequestedMoreItemsThanHaveInStockEvent) =
    result {
        let! nonNullDto = dto |> NullableDto'.ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryId'.ofDTO "InventoryId"
        let! name = nonNullDto.Name |> InventoryName'.ofDTO "Name"
        let! stockQuantity = nonNullDto.StockQuantity |> StockQuantity'.ofDTO "StockQuantity"
        let! requestedCount = nonNullDto.RequestedCount |> Count'.ofDTO "RequestedCount"

        return
            { RequestedMoreItemsThanHaveInStock.InventoryId = inventoryId
              Name = name
              StockQuantity = stockQuantity
              RequestedCount = requestedCount }
    }

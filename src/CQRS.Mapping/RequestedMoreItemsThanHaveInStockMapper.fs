module CQRS.Mapping.RequestedMoreItemsThanHaveInStockMapper

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.NullableDtoMapper
open FsToolkit.ErrorHandling

let toDTO (domain: RequestedMoreItemsThanHaveInStock) =
    let dto = RequestedMoreItemsThanHaveInStockEvent()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.toDTO
    dto.Name <- domain.Name |> InventoryNameMapper.toDTO
    dto.StockQuantity <- domain.StockQuantity |> StockQuantityMapper.toDTO
    dto.RequestedCount <- domain.RequestedCount |> CountMapper.toDTO
    dto

let ofDTO (dto: RequestedMoreItemsThanHaveInStockEvent) =
    result {
        let! nonNullDto = dto |> ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.ofDTO "InventoryId"
        let! name = nonNullDto.Name |> InventoryNameMapper.ofDTO "Name"
        let! stockQuantity = nonNullDto.StockQuantity |> StockQuantityMapper.ofDTO "StockQuantity"
        let! requestedCount = nonNullDto.RequestedCount |> CountMapper.ofDTO "RequestedCount"

        return
            { RequestedMoreItemsThanHaveInStock.InventoryId = inventoryId
              Name = name
              StockQuantity = stockQuantity
              RequestedCount = requestedCount }
    }

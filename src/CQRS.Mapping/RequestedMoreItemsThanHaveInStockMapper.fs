module CQRS.Mapping.RequestedMoreItemsThanHaveInStockMapper

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.DtoMapper
open FsToolkit.ErrorHandling

let fromDomain (domain: RequestedMoreItemsThanHaveInStock) =
    let dto = RequestedMoreItemsThanHaveInStockEvent()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.fromDomain
    dto.Name <- domain.Name |> InventoryNameMapper.fromDomain
    dto.StockQuantity <- domain.StockQuantity |> StockQuantityMapper.fromDomain
    dto.RequestedCount <- domain.RequestedCount |> CountMapper.fromDomain
    dto

let toDomain (dto: RequestedMoreItemsThanHaveInStockEvent) =
    result {
        let! nonNullDto = dto |> ensureNotNull
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.toDomain "InventoryId"
        let! name = nonNullDto.Name |> InventoryNameMapper.toDomain "Name"
        let! stockQuantity = nonNullDto.StockQuantity |> StockQuantityMapper.toDomain "StockQuantity"
        let! requestedCount = nonNullDto.RequestedCount |> CountMapper.toDomain "RequestedCount"

        return
            { RequestedMoreItemsThanHaveInStock.InventoryId = inventoryId
              Name = name
              StockQuantity = stockQuantity
              RequestedCount = requestedCount }
    }

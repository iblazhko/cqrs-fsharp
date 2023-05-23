module CQRS.Mapping.ItemInStockMapper

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.DtoMapper
open FsToolkit.ErrorHandling

let fromDomain (domain: ItemInStock) =
    let dto = ItemInStockEvent()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.fromDomain
    dto.Name <- domain.Name |> InventoryNameMapper.fromDomain
    dto.StockQuantity <- domain.StockQuantity |> StockQuantityMapper.fromDomain
    dto

let toDomain (dto: ItemInStockEvent) =
    result {
        let! nonNullDto = dto |> ensureNotNull
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.toDomain "InventoryId"
        let! name = nonNullDto.Name |> InventoryNameMapper.toDomain "Name"
        let! stockQuantity = nonNullDto.StockQuantity |> StockQuantityMapper.toDomain "StockQuantity"

        return
            { ItemInStock.InventoryId = inventoryId
              Name = name
              StockQuantity = stockQuantity }
    }

module CQRS.Mapping.ItemInStockMapper

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.NullableDtoMapper
open FsToolkit.ErrorHandling

let toDTO (domain: ItemInStock) =
    let dto = ItemInStockEvent()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.toDTO
    dto.Name <- domain.Name |> InventoryNameMapper.toDTO
    dto.StockQuantity <- domain.StockQuantity |> StockQuantityMapper.toDTO
    dto

let ofDTO (dto: ItemInStockEvent) =
    result {
        let! nonNullDto = dto |> ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.ofDTO "InventoryId"
        let! name = nonNullDto.Name |> InventoryNameMapper.ofDTO "Name"
        let! stockQuantity = nonNullDto.StockQuantity |> StockQuantityMapper.ofDTO "StockQuantity"

        return
            { ItemInStock.InventoryId = inventoryId
              Name = name
              StockQuantity = stockQuantity }
    }

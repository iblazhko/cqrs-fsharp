module CQRS.Mapping.ItemWentOutOfStockMapper

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.NullableDtoMapper
open FsToolkit.ErrorHandling

let toDTO (domain: ItemWentOutOfStock) =
    let dto = ItemWentOutOfStockEvent()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.toDTO
    dto.Name <- domain.Name |> InventoryNameMapper.toDTO
    dto

let ofDTO (dto: ItemWentOutOfStockEvent) =
    result {
        let! nonNullDto = dto |> ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.ofDTO "InventoryId"
        let! name = nonNullDto.Name |> InventoryNameMapper.ofDTO "Name"

        return
            { ItemWentOutOfStock.InventoryId = inventoryId
              Name = name }
    }

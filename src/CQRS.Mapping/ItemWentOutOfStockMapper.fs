module CQRS.Mapping.ItemWentOutOfStockMapper

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.DtoMapper
open FsToolkit.ErrorHandling

let fromDomain (domain: ItemWentOutOfStock) =
    let dto = ItemWentOutOfStockEvent()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.fromDomain
    dto.Name <- domain.Name |> InventoryNameMapper.fromDomain
    dto

let toDomain (dto: ItemWentOutOfStockEvent) =
    result {
        let! nonNullDto = dto |> ensureNotNull
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.toDomain "InventoryId"
        let! name = nonNullDto.Name |> InventoryNameMapper.toDomain "Name"

        return
            { ItemWentOutOfStock.InventoryId = inventoryId
              Name = name }
    }

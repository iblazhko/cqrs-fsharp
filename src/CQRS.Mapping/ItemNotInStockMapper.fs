module CQRS.Mapping.ItemNotInStockMapper

open CQRS.Domain.Inventory
open CQRS.Domain
open CQRS.DTO.V1
open CQRS.Mapping.DtoMapper
open FsToolkit.ErrorHandling

let fromDomain (domain: Inventory.ItemNotInStock) =
    let dto = ItemNotInStockEvent()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.fromDomain
    dto.Name <- domain.Name |> InventoryNameMapper.fromDomain
    dto

let toDomain (dto: ItemNotInStockEvent) =
    result {
        let! nonNullDto = dto |> ensureNotNull
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.toDomain "InventoryId"
        let! name = nonNullDto.Name |> InventoryNameMapper.toDomain "Name"

        return
            { ItemNotInStock.InventoryId = inventoryId
              Name = name }
    }

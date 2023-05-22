module CQRS.Mapping.InventoryItemCreatedMapper

open CQRS.Domain
open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.DtoMapper
open FsToolkit.ErrorHandling

let fromDomain (domain: Inventory.InventoryItemCreated) =
    let dto = InventoryItemCreatedEvent()
    dto.InventoryItemId <- domain.InventoryItemId |> InventoryItemIdMapper.fromDomain
    dto.Name <- domain.Name |> InventoryItemNameMapper.fromDomain
    dto

let toDomain (dto: InventoryItemCreatedEvent) =
    result {
        let! nonNullDto = dto |> ensureNotNull
        let! inventoryItemId = nonNullDto.InventoryItemId |> InventoryItemIdMapper.toDomain "InventoryItemId"
        let! name = nonNullDto.Name |> InventoryItemNameMapper.toDomain "Name"

        return
            { InventoryItemCreated.InventoryItemId = inventoryItemId
              Name = name }
    }

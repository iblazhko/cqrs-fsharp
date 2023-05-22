module CQRS.Mapping.InventoryItemDeactivatedMapper

open CQRS.Domain
open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.DtoMapper
open FsToolkit.ErrorHandling

let fromDomain (domain: Inventory.InventoryItemDeactivated) =
    let dto = InventoryItemDeactivatedEvent()
    dto.InventoryItemId <- domain.InventoryItemId |> InventoryItemIdMapper.fromDomain
    dto.Name <- domain.Name |> InventoryItemNameMapper.fromDomain
    dto

let toDomain (dto: InventoryItemDeactivatedEvent) =
    result {
        let! nonNullDto = dto |> ensureNotNull
        let! inventoryItemId = nonNullDto.InventoryItemId |> InventoryItemIdMapper.toDomain "InventoryItemId"
        let! name = nonNullDto.Name |> InventoryItemNameMapper.toDomain "Name"

        return
            { InventoryItemDeactivated.InventoryItemId = inventoryItemId
              Name = name }
    }

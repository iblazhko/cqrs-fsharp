module CQRS.Mapping.InventoryCreatedMapper

open CQRS.Domain
open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.DtoMapper
open FsToolkit.ErrorHandling

let fromDomain (domain: Inventory.InventoryCreated) =
    let dto = InventoryCreatedEvent()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.fromDomain
    dto.Name <- domain.Name |> InventoryNameMapper.fromDomain
    dto.IsActive <- domain.IsActive
    dto

let toDomain (dto: InventoryCreatedEvent) =
    result {
        let! nonNullDto = dto |> ensureNotNull
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.toDomain "InventoryId"
        let! name = nonNullDto.Name |> InventoryNameMapper.toDomain "Name"

        return
            { InventoryCreated.InventoryId = inventoryId
              Name = name
              IsActive = dto.IsActive }
    }

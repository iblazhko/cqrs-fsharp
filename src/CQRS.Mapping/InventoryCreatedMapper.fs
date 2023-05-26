module CQRS.Mapping.InventoryCreatedMapper

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.NullableDtoMapper
open FsToolkit.ErrorHandling

let toDTO (domain: InventoryCreated) =
    let dto = InventoryCreatedEvent()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.toDTO
    dto.Name <- domain.Name |> InventoryNameMapper.toDTO
    dto.IsActive <- domain.IsActive
    dto

let ofDTO (dto: InventoryCreatedEvent) =
    result {
        let! nonNullDto = dto |> ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.ofDTO "InventoryId"
        let! name = nonNullDto.Name |> InventoryNameMapper.ofDTO "Name"

        return
            { InventoryCreated.InventoryId = inventoryId
              Name = name
              IsActive = dto.IsActive }
    }

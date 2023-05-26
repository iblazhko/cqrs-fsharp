module CQRS.Mapping.InventoryDeactivatedMapper

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.NullableDtoMapper
open FsToolkit.ErrorHandling

let toDTO (domain: InventoryDeactivated) =
    let dto = InventoryDeactivatedEvent()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.toDTO
    dto.Name <- domain.Name |> InventoryNameMapper.toDTO
    dto

let ofDTO (dto: InventoryDeactivatedEvent) =
    result {
        let! nonNullDto = dto |> ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.ofDTO "InventoryId"
        let! name = nonNullDto.Name |> InventoryNameMapper.ofDTO "Name"

        return
            { InventoryDeactivated.InventoryId = inventoryId
              Name = name }
    }

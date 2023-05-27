module CQRS.Mapping.InventoryCreated'

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open FsToolkit.ErrorHandling

let toDTO (domain: InventoryCreated) =
    let dto = InventoryCreatedEvent()
    dto.InventoryId <- domain.InventoryId |> InventoryId'.toDTO
    dto.Name <- domain.Name |> InventoryName'.toDTO
    dto.IsActive <- domain.IsActive
    dto

let ofDTO (dto: InventoryCreatedEvent) =
    result {
        let! nonNullDto = dto |> NullableDto'.ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryId'.ofDTO "InventoryId"
        let! name = nonNullDto.Name |> InventoryName'.ofDTO "Name"

        return
            { InventoryCreated.InventoryId = inventoryId
              Name = name
              IsActive = dto.IsActive }
    }

module CQRS.Mapping.InventoryDeactivated'

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open FsToolkit.ErrorHandling

let toDTO (domain: InventoryDeactivated) =
    let dto = InventoryDeactivatedEvent()
    dto.InventoryId <- domain.InventoryId |> InventoryId'.toDTO
    dto.Name <- domain.Name |> InventoryName'.toDTO
    dto

let ofDTO (dto: InventoryDeactivatedEvent) =
    result {
        let! nonNullDto = dto |> NullableDto'.ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryId'.ofDTO "InventoryId"
        let! name = nonNullDto.Name |> InventoryName'.ofDTO "Name"

        return
            { InventoryDeactivated.InventoryId = inventoryId
              Name = name }
    }

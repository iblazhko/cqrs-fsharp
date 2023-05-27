module CQRS.Mapping.DeactivateInventory'

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open FsToolkit.ErrorHandling

let toDTO (domain: DeactivateInventory) =
    let dto = DeactivateInventoryCommand()
    dto.InventoryId <- domain.InventoryId |> InventoryId'.toDTO
    dto

let ofDTO (dto: DeactivateInventoryCommand) =
    result {
        let! nonNullDto = dto |> NullableDto'.ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryId'.ofDTO "InventoryId"
        return { DeactivateInventory.InventoryId = inventoryId }
    }

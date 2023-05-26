module CQRS.Mapping.DeactivateInventoryMapper

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.NullableDtoMapper
open FsToolkit.ErrorHandling

let toDTO (domain: DeactivateInventory) =
    let dto = DeactivateInventoryCommand()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.toDTO
    dto

let ofDTO (dto: DeactivateInventoryCommand) =
    result {
        let! nonNullDto = dto |> ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.ofDTO "InventoryId"
        return { DeactivateInventory.InventoryId = inventoryId }
    }

module CQRS.Mapping.AddItemsToInventoryMapper

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.NullableDtoMapper
open FsToolkit.ErrorHandling

let toDTO (domain: AddItemsToInventory) =
    let dto = AddItemsToInventoryCommand()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.toDTO
    dto.Count <- domain.Count |> CountMapper.toDTO
    dto

let ofDTO (dto: AddItemsToInventoryCommand) =
    result {
        let! nonNullDto = dto |> ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.ofDTO "InventoryId"
        let! count = nonNullDto.Count |> CountMapper.ofDTO "Count"

        return
            { AddItemsToInventory.InventoryId = inventoryId
              Count = count }
    }

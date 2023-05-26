module CQRS.Mapping.RemoveItemsFromInventoryMapper

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.NullableDtoMapper
open FsToolkit.ErrorHandling

let toDTO (domain: RemoveItemsFromInventory) =
    let dto = RemoveItemsFromInventoryCommand()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.toDTO
    dto.Count <- domain.Count |> CountMapper.toDTO
    dto

let ofDTO (dto: RemoveItemsFromInventoryCommand) =
    result {
        let! nonNullDto = dto |> ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.ofDTO "InventoryId"
        let! count = nonNullDto.Count |> CountMapper.ofDTO "Count"

        return
            { RemoveItemsFromInventory.InventoryId = inventoryId
              Count = count }
    }

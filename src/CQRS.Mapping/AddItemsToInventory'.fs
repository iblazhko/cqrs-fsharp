module CQRS.Mapping.AddItemsToInventory'

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open FsToolkit.ErrorHandling

let toDTO (domain: AddItemsToInventory) =
    let dto = AddItemsToInventoryCommand()
    dto.InventoryId <- domain.InventoryId |> InventoryId'.toDTO
    dto.Count <- domain.Count |> Count'.toDTO
    dto

let ofDTO (dto: AddItemsToInventoryCommand) =
    result {
        let! nonNullDto = dto |> NullableDto'.ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryId'.ofDTO "InventoryId"
        let! count = nonNullDto.Count |> Count'.ofDTO "Count"

        return
            { AddItemsToInventory.InventoryId = inventoryId
              Count = count }
    }

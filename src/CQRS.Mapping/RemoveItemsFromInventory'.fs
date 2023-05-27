module CQRS.Mapping.RemoveItemsFromInventory'

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open FsToolkit.ErrorHandling

let toDTO (domain: RemoveItemsFromInventory) =
    let dto = RemoveItemsFromInventoryCommand()
    dto.InventoryId <- domain.InventoryId |> InventoryId'.toDTO
    dto.Count <- domain.Count |> Count'.toDTO
    dto

let ofDTO (dto: RemoveItemsFromInventoryCommand) =
    result {
        let! nonNullDto = dto |> NullableDto'.ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryId'.ofDTO "InventoryId"
        let! count = nonNullDto.Count |> Count'.ofDTO "Count"

        return
            { RemoveItemsFromInventory.InventoryId = inventoryId
              Count = count }
    }

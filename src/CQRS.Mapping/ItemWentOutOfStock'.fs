module CQRS.Mapping.ItemWentOutOfStock'

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open FsToolkit.ErrorHandling

let toDTO (domain: ItemWentOutOfStock) =
    let dto = ItemWentOutOfStockEvent()
    dto.InventoryId <- domain.InventoryId |> InventoryId'.toDTO
    dto.Name <- domain.Name |> InventoryName'.toDTO
    dto

let ofDTO (dto: ItemWentOutOfStockEvent) =
    result {
        let! nonNullDto = dto |> NullableDto'.ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryId'.ofDTO "InventoryId"
        let! name = nonNullDto.Name |> InventoryName'.ofDTO "Name"

        return
            { ItemWentOutOfStock.InventoryId = inventoryId
              Name = name }
    }

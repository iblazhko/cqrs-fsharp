module CQRS.Mapping.RemoveItemsFromInventoryMapper

open CQRS.Domain
open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.DtoMapper
open FsToolkit.ErrorHandling

let fromDomain (domain: Inventory.RemoveItemsFromInventory) =
    let dto = RemoveItemsFromInventoryCommand()
    dto.InventoryItemId <- domain.InventoryItemId |> InventoryItemIdMapper.fromDomain
    dto.Count <- domain.Count |> CountMapper.fromDomain
    dto

let toDomain (dto: RemoveItemsFromInventoryCommand) =
    result {
        let! nonNullDto = dto |> ensureNotNull
        let! inventoryItemId = nonNullDto.InventoryItemId |> InventoryItemIdMapper.toDomain "InventoryItemId"
        let! count = nonNullDto.Count |> CountMapper.toDomain "Count"

        return
            { RemoveItemsFromInventory.InventoryItemId = inventoryItemId
              Count = count }
    }

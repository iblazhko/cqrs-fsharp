module CQRS.Mapping.RemoveItemsFromInventoryMapper

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.DtoMapper
open FsToolkit.ErrorHandling

let fromDomain (domain: RemoveItemsFromInventory) =
    let dto = RemoveItemsFromInventoryCommand()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.fromDomain
    dto.Count <- domain.Count |> CountMapper.fromDomain
    dto

let toDomain (dto: RemoveItemsFromInventoryCommand) =
    result {
        let! nonNullDto = dto |> ensureNotNull
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.toDomain "InventoryId"
        let! count = nonNullDto.Count |> CountMapper.toDomain "Count"

        return
            { RemoveItemsFromInventory.InventoryId = inventoryId
              Count = count }
    }

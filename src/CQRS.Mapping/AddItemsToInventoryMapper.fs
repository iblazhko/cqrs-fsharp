module CQRS.Mapping.AddItemsToInventoryMapper

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.DtoMapper
open FsToolkit.ErrorHandling

let fromDomain (domain: AddItemsToInventory) =
    let dto = AddItemsToInventoryCommand()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.fromDomain
    dto.Count <- domain.Count |> CountMapper.fromDomain
    dto

let toDomain (dto: AddItemsToInventoryCommand) =
    result {
        let! nonNullDto = dto |> ensureNotNull
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.toDomain "InventoryId"
        let! count = nonNullDto.Count |> CountMapper.toDomain "Count"

        return
            { AddItemsToInventory.InventoryId = inventoryId
              Count = count }
    }

module CQRS.Mapping.InventoryItemStateMapper

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.DtoMapper
open FsToolkit.ErrorHandling

let fromDomain (domain: InventoryItemState) =
    let dto = InventoryItem()
    dto.InventoryItemId <- domain.InventoryItemId |> InventoryItemIdMapper.fromDomain
    dto.Name <- domain.Name |> InventoryItemNameMapper.fromDomain
    dto.StockQuantity <- domain.StockQuantity |> StockQuantityMapper.fromDomain
    dto.IsNew <- domain.IsNew
    dto.IsActive <- domain.IsActive

let toDomain (dto: InventoryItem) =
    result {
        let! nonNullDto = dto |> ensureNotNull
        let! inventoryItemId = nonNullDto.InventoryItemId |> InventoryItemIdMapper.toDomain "InventoryItemId"
        let! name = nonNullDto.Name |> InventoryItemNameMapper.toDomain "Name"
        let! stockQuantity = nonNullDto.StockQuantity |> StockQuantityMapper.toDomain "StockQuantity"

        return
            { InventoryItemId = inventoryItemId
              Name = name
              StockQuantity = stockQuantity
              IsNew = dto.IsNew
              IsActive = dto.IsActive }
    }

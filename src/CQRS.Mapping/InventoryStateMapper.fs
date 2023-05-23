module CQRS.Mapping.InventoryStateMapper

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.DtoMapper
open FsToolkit.ErrorHandling

let fromDomain (domain: InventoryState) =
    let dto = Inventory()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.fromDomain
    dto.Name <- domain.Name |> InventoryNameMapper.fromDomain
    dto.StockQuantity <- domain.StockQuantity |> StockQuantityMapper.fromDomain
    dto.IsNew <- domain.IsNew
    dto.IsActive <- domain.IsActive

let toDomain (dto: Inventory) =
    result {
        let! nonNullDto = dto |> ensureNotNull
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.toDomain "InventoryId"
        let! name = nonNullDto.Name |> InventoryNameMapper.toDomain "Name"
        let! stockQuantity = nonNullDto.StockQuantity |> StockQuantityMapper.toDomain "StockQuantity"

        return
            { InventoryId = inventoryId
              Name = name
              StockQuantity = stockQuantity
              IsNew = dto.IsNew
              IsActive = dto.IsActive }
    }

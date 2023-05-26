module CQRS.Mapping.InventoryStateMapper

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.NullableDtoMapper
open FsToolkit.ErrorHandling

let toDTO (domain: InventoryState) =
    let dto = Inventory()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.toDTO
    dto.Name <- domain.Name |> InventoryNameMapper.toDTO
    dto.StockQuantity <- domain.StockQuantity |> StockQuantityMapper.toDTO
    dto.IsNew <- domain.IsNew
    dto.IsActive <- domain.IsActive

let ofDTO (dto: Inventory) =
    result {
        let! nonNullDto = dto |> ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.ofDTO "InventoryId"
        let! name = nonNullDto.Name |> InventoryNameMapper.ofDTO "Name"
        let! stockQuantity = nonNullDto.StockQuantity |> StockQuantityMapper.ofDTO "StockQuantity"

        return
            { InventoryState.InventoryId = inventoryId
              Name = name
              StockQuantity = stockQuantity
              IsNew = dto.IsNew
              IsActive = dto.IsActive }
    }

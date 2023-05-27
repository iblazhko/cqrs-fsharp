module CQRS.Mapping.InventoryState'

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open FsToolkit.ErrorHandling

let toDTO (domain: InventoryState) =
    let dto = Inventory()
    dto.InventoryId <- domain.InventoryId |> InventoryId'.toDTO
    dto.Name <- domain.Name |> InventoryName'.toDTO
    dto.StockQuantity <- domain.StockQuantity |> StockQuantity'.toDTO
    dto.IsNew <- domain.IsNew
    dto.IsActive <- domain.IsActive

let ofDTO (dto: Inventory) =
    result {
        let! nonNullDto = dto |> NullableDto'.ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryId'.ofDTO "InventoryId"
        let! name = nonNullDto.Name |> InventoryName'.ofDTO "Name"
        let! stockQuantity = nonNullDto.StockQuantity |> StockQuantity'.ofDTO "StockQuantity"

        return
            { InventoryState.InventoryId = inventoryId
              Name = name
              StockQuantity = stockQuantity
              IsNew = dto.IsNew
              IsActive = dto.IsActive }
    }

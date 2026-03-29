module CQRS.Mapping.InventoryState'

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open FsToolkit.ErrorHandling

let toDTO (domain: InventoryState) =
    let dto = Inventory()
    match domain with
    | Uninitialized id ->
        dto.InventoryId <- id |> InventoryId'.toDTO
        dto.IsNew <- true
        dto.IsActive <- true
    | Active data ->
        dto.InventoryId <- data.InventoryId |> InventoryId'.toDTO
        dto.Name <- data.Name |> InventoryName'.toDTO
        dto.StockQuantity <- data.StockQuantity |> StockQuantity'.toDTO
        dto.IsActive <- true
    | Inactive data ->
        dto.InventoryId <- data.InventoryId |> InventoryId'.toDTO
        dto.Name <- data.Name |> InventoryName'.toDTO
        dto.StockQuantity <- data.StockQuantity |> StockQuantity'.toDTO
        dto.IsActive <- false
    dto

let ofDTO (dto: Inventory) =
    result {
        let! nonNullDto = dto |> NullableDto'.ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryId'.ofDTO "InventoryId"
        let! name = nonNullDto.Name |> InventoryName'.ofDTO "Name"
        let! stockQuantity = nonNullDto.StockQuantity |> StockQuantity'.ofDTO "StockQuantity"

        return
            if nonNullDto.IsNew then
                Uninitialized inventoryId
            elif nonNullDto.IsActive then
                Active { InventoryId = inventoryId; Name = name; StockQuantity = stockQuantity }
            else
                Inactive { InventoryId = inventoryId; Name = name; StockQuantity = stockQuantity }
    }

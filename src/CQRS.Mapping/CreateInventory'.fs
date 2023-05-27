module CQRS.Mapping.CreateInventory'

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open FsToolkit.ErrorHandling

let toDTO (domain: CreateInventory) =
    let dto = CreateInventoryCommand()
    dto.InventoryId <- domain.InventoryId |> InventoryId'.toDTO
    dto.Name <- domain.Name |> InventoryName'.toDTO
    dto

let ofDTO (dto: CreateInventoryCommand) =
    result {
        let! nonNullDto = dto |> NullableDto'.ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryId'.ofDTO "InventoryId"
        let! inventoryName = nonNullDto.Name |> InventoryName'.ofDTO "Name"

        return
            { CreateInventory.InventoryId = inventoryId
              Name = inventoryName }
    }

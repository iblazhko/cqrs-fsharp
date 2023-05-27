module CQRS.Mapping.RenameInventory'

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open FsToolkit.ErrorHandling

let toDTO (domain: RenameInventory) =
    let dto = RenameInventoryCommand()
    dto.InventoryId <- domain.InventoryId |> InventoryId'.toDTO
    dto.NewName <- domain.NewName |> InventoryName'.toDTO
    dto

let ofDTO (dto: RenameInventoryCommand) =
    result {
        let! nonNullDto = dto |> NullableDto'.ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryId'.ofDTO "InventoryId"
        let! newName = nonNullDto.NewName |> InventoryName'.ofDTO "NewName"

        return
            { RenameInventory.InventoryId = inventoryId
              NewName = newName }
    }

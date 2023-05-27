module CQRS.Mapping.InventoryRenamed'

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open FsToolkit.ErrorHandling

let toDTO (domain: InventoryRenamed) =
    let dto = InventoryRenamedEvent()
    dto.InventoryId <- domain.InventoryId |> InventoryId'.toDTO
    dto.OldName <- domain.OldName |> InventoryName'.toDTO
    dto.NewName <- domain.NewName |> InventoryName'.toDTO
    dto

let ofDTO (dto: InventoryRenamedEvent) =
    result {
        let! nonNullDto = dto |> NullableDto'.ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryId'.ofDTO "InventoryId"
        let! oldName = nonNullDto.OldName |> InventoryName'.ofDTO "OldName"
        let! newName = nonNullDto.NewName |> InventoryName'.ofDTO "NewName"

        return
            { InventoryRenamed.InventoryId = inventoryId
              OldName = oldName
              NewName = newName }
    }

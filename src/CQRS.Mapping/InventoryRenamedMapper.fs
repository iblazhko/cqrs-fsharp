module CQRS.Mapping.InventoryRenamedMapper

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.NullableDtoMapper
open FsToolkit.ErrorHandling

let toDTO (domain: InventoryRenamed) =
    let dto = InventoryRenamedEvent()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.toDTO
    dto.OldName <- domain.OldName |> InventoryNameMapper.toDTO
    dto.NewName <- domain.NewName |> InventoryNameMapper.toDTO
    dto

let ofDTO (dto: InventoryRenamedEvent) =
    result {
        let! nonNullDto = dto |> ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.ofDTO "InventoryId"
        let! oldName = nonNullDto.OldName |> InventoryNameMapper.ofDTO "OldName"
        let! newName = nonNullDto.NewName |> InventoryNameMapper.ofDTO "NewName"

        return
            { InventoryRenamed.InventoryId = inventoryId
              OldName = oldName
              NewName = newName }
    }

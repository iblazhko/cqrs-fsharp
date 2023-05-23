module CQRS.Mapping.InventoryRenamedMapper

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.DtoMapper
open FsToolkit.ErrorHandling

let fromDomain (domain: InventoryRenamed) =
    let dto = InventoryRenamedEvent()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.fromDomain
    dto.OldName <- domain.OldName |> InventoryNameMapper.fromDomain
    dto.NewName <- domain.NewName |> InventoryNameMapper.fromDomain
    dto

let toDomain (dto: InventoryRenamedEvent) =
    result {
        let! nonNullDto = dto |> ensureNotNull
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.toDomain "InventoryId"
        let! oldName = nonNullDto.OldName |> InventoryNameMapper.toDomain "OldName"
        let! newName = nonNullDto.NewName |> InventoryNameMapper.toDomain "NewName"

        return
            { InventoryRenamed.InventoryId = inventoryId
              OldName = oldName
              NewName = newName }
    }

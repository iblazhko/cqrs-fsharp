module CQRS.Mapping.RenameInventoryMapper

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.DtoMapper
open FsToolkit.ErrorHandling

let fromDomain (domain: RenameInventory) =
    let dto = RenameInventoryCommand()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.fromDomain
    dto.NewName <- domain.NewName |> InventoryNameMapper.fromDomain
    dto

let toDomain (dto: RenameInventoryCommand) =
    result {
        let! nonNullDto = dto |> ensureNotNull
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.toDomain "InventoryId"
        let! newName = nonNullDto.NewName |> InventoryNameMapper.toDomain "NewName"

        return
            { RenameInventory.InventoryId = inventoryId
              NewName = newName }
    }

module CQRS.Mapping.RenameInventoryMapper

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.NullableDtoMapper
open FsToolkit.ErrorHandling

let toDTO (domain: RenameInventory) =
    let dto = RenameInventoryCommand()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.toDTO
    dto.NewName <- domain.NewName |> InventoryNameMapper.toDTO
    dto

let ofDTO (dto: RenameInventoryCommand) =
    result {
        let! nonNullDto = dto |> ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.ofDTO "InventoryId"
        let! newName = nonNullDto.NewName |> InventoryNameMapper.ofDTO "NewName"

        return
            { RenameInventory.InventoryId = inventoryId
              NewName = newName }
    }

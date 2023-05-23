module CQRS.Mapping.CreateInventoryMapper

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.DtoMapper
open FsToolkit.ErrorHandling

let fromDomain (domain: CreateInventory) =
    let dto = CreateInventoryCommand()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.fromDomain
    dto.Name <- domain.Name |> InventoryNameMapper.fromDomain
    dto

let toDomain (dto: CreateInventoryCommand) =
    result {
        let! nonNullDto = dto |> ensureNotNull
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.toDomain "InventoryId"
        let! inventoryName = nonNullDto.Name |> InventoryNameMapper.toDomain "Name"

        return
            { CreateInventory.InventoryId = inventoryId
              Name = inventoryName }
    }

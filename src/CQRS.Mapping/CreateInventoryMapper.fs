module CQRS.Mapping.CreateInventoryMapper

open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping.NullableDtoMapper
open FsToolkit.ErrorHandling

let toDTO (domain: CreateInventory) =
    let dto = CreateInventoryCommand()
    dto.InventoryId <- domain.InventoryId |> InventoryIdMapper.toDTO
    dto.Name <- domain.Name |> InventoryNameMapper.toDTO
    dto

let ofDTO (dto: CreateInventoryCommand) =
    result {
        let! nonNullDto = dto |> ofNullable
        let! inventoryId = nonNullDto.InventoryId |> InventoryIdMapper.ofDTO "InventoryId"
        let! inventoryName = nonNullDto.Name |> InventoryNameMapper.ofDTO "Name"

        return
            { CreateInventory.InventoryId = inventoryId
              Name = inventoryName }
    }

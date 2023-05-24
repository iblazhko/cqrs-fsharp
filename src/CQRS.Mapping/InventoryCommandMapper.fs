module CQRS.Mapping.InventoryCommandMapper

open CQRS.DTO
open CQRS.DTO.V1
open CQRS.Domain.Inventory

exception CommandDtoMappingException of string

let fromDomain (cmd: InventoryCommand) =
    match cmd with
    | CreateInventory x -> x |> CreateInventoryMapper.fromDomain :> CqrsCommandDto
    | RenameInventory x -> x |> RenameInventoryMapper.fromDomain :> CqrsCommandDto
    | AddItemsToInventory x -> x |> AddItemsToInventoryMapper.fromDomain :> CqrsCommandDto
    | RemoveItemsFromInventory x -> x |> RemoveItemsFromInventoryMapper.fromDomain :> CqrsCommandDto
    | DeactivateInventory x -> x |> DeactivateInventoryMapper.fromDomain :> CqrsCommandDto

let toDomain (dto: CqrsCommandDto) =
    match dto with
    | :? CreateInventoryCommand ->
        (dto :?> CreateInventoryCommand)
        |> CreateInventoryMapper.toDomain
        |> Result.map InventoryCommand.CreateInventory
    | :? RenameInventoryCommand ->
        (dto :?> RenameInventoryCommand)
        |> RenameInventoryMapper.toDomain
        |> Result.map InventoryCommand.RenameInventory
    | :? AddItemsToInventoryCommand ->
        (dto :?> AddItemsToInventoryCommand)
        |> AddItemsToInventoryMapper.toDomain
        |> Result.map InventoryCommand.AddItemsToInventory
    | :? RemoveItemsFromInventoryCommand ->
        (dto :?> RemoveItemsFromInventoryCommand)
        |> RemoveItemsFromInventoryMapper.toDomain
        |> Result.map InventoryCommand.RemoveItemsFromInventory
    | :? DeactivateInventoryCommand ->
        (dto :?> DeactivateInventoryCommand)
        |> DeactivateInventoryMapper.toDomain
        |> Result.map InventoryCommand.DeactivateInventory
    | x -> raise (CommandDtoMappingException $"Unknown command DTO type: {x.GetType().FullName}")

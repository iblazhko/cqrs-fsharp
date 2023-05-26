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
    | :? CreateInventoryCommand as x ->
        x
        |> CreateInventoryMapper.toDomain
        |> Result.map InventoryCommand.CreateInventory
    | :? RenameInventoryCommand as x ->
        x
        |> RenameInventoryMapper.toDomain
        |> Result.map InventoryCommand.RenameInventory
    | :? AddItemsToInventoryCommand as x ->
        x
        |> AddItemsToInventoryMapper.toDomain
        |> Result.map InventoryCommand.AddItemsToInventory
    | :? RemoveItemsFromInventoryCommand as x ->
        x
        |> RemoveItemsFromInventoryMapper.toDomain
        |> Result.map InventoryCommand.RemoveItemsFromInventory
    | :? DeactivateInventoryCommand as x ->
        x
        |> DeactivateInventoryMapper.toDomain
        |> Result.map InventoryCommand.DeactivateInventory
    | x -> raise (CommandDtoMappingException $"Unknown command DTO type: {x.GetType().FullName}")

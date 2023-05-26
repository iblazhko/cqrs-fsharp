module CQRS.Mapping.InventoryCommandMapper

open CQRS.DTO
open CQRS.DTO.V1
open CQRS.Domain.Inventory

exception CommandDtoMappingException of string

let toDTO (cmd: InventoryCommand) =
    match cmd with
    | CreateInventory x -> x |> CreateInventoryMapper.toDTO :> CqrsCommandDto
    | RenameInventory x -> x |> RenameInventoryMapper.toDTO :> CqrsCommandDto
    | AddItemsToInventory x -> x |> AddItemsToInventoryMapper.toDTO :> CqrsCommandDto
    | RemoveItemsFromInventory x -> x |> RemoveItemsFromInventoryMapper.toDTO :> CqrsCommandDto
    | DeactivateInventory x -> x |> DeactivateInventoryMapper.toDTO :> CqrsCommandDto

let ofDTO (dto: CqrsCommandDto) =
    match dto with
    | :? CreateInventoryCommand as x -> x |> CreateInventoryMapper.ofDTO |> Result.map InventoryCommand.CreateInventory
    | :? RenameInventoryCommand as x -> x |> RenameInventoryMapper.ofDTO |> Result.map InventoryCommand.RenameInventory
    | :? AddItemsToInventoryCommand as x ->
        x
        |> AddItemsToInventoryMapper.ofDTO
        |> Result.map InventoryCommand.AddItemsToInventory
    | :? RemoveItemsFromInventoryCommand as x ->
        x
        |> RemoveItemsFromInventoryMapper.ofDTO
        |> Result.map InventoryCommand.RemoveItemsFromInventory
    | :? DeactivateInventoryCommand as x ->
        x
        |> DeactivateInventoryMapper.ofDTO
        |> Result.map InventoryCommand.DeactivateInventory
    | x -> raise (CommandDtoMappingException $"Unknown command DTO type: {x.GetType().FullName}")

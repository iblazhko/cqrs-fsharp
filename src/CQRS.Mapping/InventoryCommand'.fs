module CQRS.Mapping.InventoryCommand'

open System
open CQRS.DTO
open CQRS.DTO.V1
open CQRS.Domain.Inventory

exception CommandDtoMappingException of string

let toDTO (cmd: InventoryCommand) =
    match cmd with
    | CreateInventory x -> x |> CreateInventory'.toDTO :> CqrsCommandDto
    | RenameInventory x -> x |> RenameInventory'.toDTO :> CqrsCommandDto
    | AddItemsToInventory x -> x |> AddItemsToInventory'.toDTO :> CqrsCommandDto
    | RemoveItemsFromInventory x -> x |> RemoveItemsFromInventory'.toDTO :> CqrsCommandDto
    | DeactivateInventory x -> x |> DeactivateInventory'.toDTO :> CqrsCommandDto

let ofDTO (dto: CqrsCommandDto) =
    match dto with
    | x when (isNull x) -> raise (ArgumentNullException("DTO instance is required", (nameof dto)))
    | :? CreateInventoryCommand as x -> x |> CreateInventory'.ofDTO |> Result.map InventoryCommand.CreateInventory
    | :? RenameInventoryCommand as x -> x |> RenameInventory'.ofDTO |> Result.map InventoryCommand.RenameInventory
    | :? AddItemsToInventoryCommand as x ->
        x
        |> AddItemsToInventory'.ofDTO
        |> Result.map InventoryCommand.AddItemsToInventory
    | :? RemoveItemsFromInventoryCommand as x ->
        x
        |> RemoveItemsFromInventory'.ofDTO
        |> Result.map InventoryCommand.RemoveItemsFromInventory
    | :? DeactivateInventoryCommand as x ->
        x
        |> DeactivateInventory'.ofDTO
        |> Result.map InventoryCommand.DeactivateInventory
    | x -> raise (CommandDtoMappingException $"Unknown command DTO type: {x.GetType().FullName}")

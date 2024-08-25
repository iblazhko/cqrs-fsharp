module CQRS.Mapping.InventoryEvent'

open System
open CQRS.DTO
open CQRS.DTO.V1
open CQRS.Domain.Inventory

exception EventDtoMappingException of string

let toDTO (evt: InventoryEvent) =
    match evt with
    | InventoryCreated x -> x |> InventoryCreated'.toDTO :> CqrsEventDto
    | InventoryRenamed x -> x |> InventoryRenamed'.toDTO :> CqrsEventDto
    | ItemsAddedToInventory x -> x |> ItemsAddedToInventory'.toDTO :> CqrsEventDto
    | ItemsRemovedFromInventory x -> x |> ItemsRemovedFromInventory'.toDTO :> CqrsEventDto
    | ItemInStock x -> x |> ItemInStock'.toDTO :> CqrsEventDto
    | ItemWentOutOfStock x -> x |> ItemWentOutOfStock'.toDTO :> CqrsEventDto
    | InventoryDeactivated x -> x |> InventoryDeactivated'.toDTO :> CqrsEventDto

let ofDTO (dto: CqrsEventDto) =
    match dto with
    | x when (isNull x) -> raise (ArgumentNullException("DTO instance is required", (nameof dto)))
    | :? InventoryCreatedEvent as x -> x |> InventoryCreated'.ofDTO |> Result.map InventoryEvent.InventoryCreated
    | :? InventoryRenamedEvent as x -> x |> InventoryRenamed'.ofDTO |> Result.map InventoryEvent.InventoryRenamed
    | :? ItemsAddedToInventoryEvent as x ->
        x
        |> ItemsAddedToInventory'.ofDTO
        |> Result.map InventoryEvent.ItemsAddedToInventory
    | :? ItemsRemovedFromInventoryEvent as x ->
        x
        |> ItemsRemovedFromInventory'.ofDTO
        |> Result.map InventoryEvent.ItemsRemovedFromInventory
    | :? ItemInStockEvent as x -> x |> ItemInStock'.ofDTO |> Result.map InventoryEvent.ItemInStock
    | :? ItemWentOutOfStockEvent as x -> x |> ItemWentOutOfStock'.ofDTO |> Result.map InventoryEvent.ItemWentOutOfStock
    | :? InventoryDeactivatedEvent as x ->
        x
        |> InventoryDeactivated'.ofDTO
        |> Result.map InventoryEvent.InventoryDeactivated
    | x -> raise (EventDtoMappingException $"Unknown event DTO type: {x.GetType().FullName}")

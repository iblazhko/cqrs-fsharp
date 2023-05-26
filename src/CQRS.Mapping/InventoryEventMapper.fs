module CQRS.Mapping.InventoryEventMapper

open CQRS.DTO
open CQRS.DTO.V1
open CQRS.Domain.Inventory

exception EventDtoMappingException of string

let fromDomain (evt: InventoryEvent) =
    match evt with
    | InventoryCreated x -> x |> InventoryCreatedMapper.fromDomain :> CqrsEventDto
    | InventoryRenamed x -> x |> InventoryRenamedMapper.fromDomain :> CqrsEventDto
    | ItemsAddedToInventory x -> x |> ItemsAddedToInventoryMapper.fromDomain :> CqrsEventDto
    | ItemsRemovedFromInventory x -> x |> ItemsRemovedFromInventoryMapper.fromDomain :> CqrsEventDto
    | ItemInStock x -> x |> ItemInStockMapper.fromDomain :> CqrsEventDto
    | ItemWentOutOfStock x -> x |> ItemWentOutOfStockMapper.fromDomain :> CqrsEventDto
    | RequestedMoreItemsThanHaveInStock x -> x |> RequestedMoreItemsThanHaveInStockMapper.fromDomain :> CqrsEventDto
    | InventoryDeactivated x -> x |> InventoryDeactivatedMapper.fromDomain :> CqrsEventDto

let toDomain (dto: CqrsEventDto) =
    match dto with
    | :? InventoryCreatedEvent as x ->
        x
        |> InventoryCreatedMapper.toDomain
        |> Result.map InventoryEvent.InventoryCreated
    | :? InventoryRenamedEvent as x ->
        x
        |> InventoryRenamedMapper.toDomain
        |> Result.map InventoryEvent.InventoryRenamed
    | :? ItemsAddedToInventoryEvent as x ->
        x
        |> ItemsAddedToInventoryMapper.toDomain
        |> Result.map InventoryEvent.ItemsAddedToInventory
    | :? ItemsRemovedFromInventoryEvent as x ->
        x
        |> ItemsRemovedFromInventoryMapper.toDomain
        |> Result.map InventoryEvent.ItemsRemovedFromInventory
    | :? ItemInStockEvent as x -> x |> ItemInStockMapper.toDomain |> Result.map InventoryEvent.ItemInStock
    | :? ItemWentOutOfStockEvent as x ->
        x
        |> ItemWentOutOfStockMapper.toDomain
        |> Result.map InventoryEvent.ItemWentOutOfStock
    | :? RequestedMoreItemsThanHaveInStockEvent as x ->
        x
        |> RequestedMoreItemsThanHaveInStockMapper.toDomain
        |> Result.map InventoryEvent.RequestedMoreItemsThanHaveInStock
    | :? InventoryDeactivatedEvent as x ->
        x
        |> InventoryDeactivatedMapper.toDomain
        |> Result.map InventoryEvent.InventoryDeactivated
    | x -> raise (EventDtoMappingException $"Unknown event DTO type: {x.GetType().FullName}")

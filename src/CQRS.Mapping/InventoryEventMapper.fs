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
    | :? InventoryCreatedEvent ->
        (dto :?> InventoryCreatedEvent)
        |> InventoryCreatedMapper.toDomain
        |> Result.map InventoryEvent.InventoryCreated
    | :? InventoryRenamedEvent ->
        (dto :?> InventoryRenamedEvent)
        |> InventoryRenamedMapper.toDomain
        |> Result.map InventoryEvent.InventoryRenamed
    | :? ItemsAddedToInventoryEvent ->
        (dto :?> ItemsAddedToInventoryEvent)
        |> ItemsAddedToInventoryMapper.toDomain
        |> Result.map InventoryEvent.ItemsAddedToInventory
    | :? ItemsRemovedFromInventoryEvent ->
        (dto :?> ItemsRemovedFromInventoryEvent)
        |> ItemsRemovedFromInventoryMapper.toDomain
        |> Result.map InventoryEvent.ItemsRemovedFromInventory
    | :? ItemInStockEvent ->
        (dto :?> ItemInStockEvent)
        |> ItemInStockMapper.toDomain
        |> Result.map InventoryEvent.ItemInStock
    | :? ItemWentOutOfStockEvent ->
        (dto :?> ItemWentOutOfStockEvent)
        |> ItemWentOutOfStockMapper.toDomain
        |> Result.map InventoryEvent.ItemWentOutOfStock
    | :? RequestedMoreItemsThanHaveInStockEvent ->
        (dto :?> RequestedMoreItemsThanHaveInStockEvent)
        |> RequestedMoreItemsThanHaveInStockMapper.toDomain
        |> Result.map InventoryEvent.RequestedMoreItemsThanHaveInStock
    | :? InventoryDeactivatedEvent ->
        (dto :?> InventoryDeactivatedEvent)
        |> InventoryDeactivatedMapper.toDomain
        |> Result.map InventoryEvent.InventoryDeactivated
    | x -> raise (EventDtoMappingException $"Unknown event DTO type: {x.GetType().FullName}")

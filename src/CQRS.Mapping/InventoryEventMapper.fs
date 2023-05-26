module CQRS.Mapping.InventoryEventMapper

open CQRS.DTO
open CQRS.DTO.V1
open CQRS.Domain.Inventory

exception EventDtoMappingException of string

let toDTO (evt: InventoryEvent) =
    match evt with
    | InventoryCreated x -> x |> InventoryCreatedMapper.toDTO :> CqrsEventDto
    | InventoryRenamed x -> x |> InventoryRenamedMapper.toDTO :> CqrsEventDto
    | ItemsAddedToInventory x -> x |> ItemsAddedToInventoryMapper.toDTO :> CqrsEventDto
    | ItemsRemovedFromInventory x -> x |> ItemsRemovedFromInventoryMapper.toDTO :> CqrsEventDto
    | ItemInStock x -> x |> ItemInStockMapper.toDTO :> CqrsEventDto
    | ItemWentOutOfStock x -> x |> ItemWentOutOfStockMapper.toDTO :> CqrsEventDto
    | RequestedMoreItemsThanHaveInStock x -> x |> RequestedMoreItemsThanHaveInStockMapper.toDTO :> CqrsEventDto
    | InventoryDeactivated x -> x |> InventoryDeactivatedMapper.toDTO :> CqrsEventDto

let ofDTO (dto: CqrsEventDto) =
    match dto with
    | :? InventoryCreatedEvent as x -> x |> InventoryCreatedMapper.ofDTO |> Result.map InventoryEvent.InventoryCreated
    | :? InventoryRenamedEvent as x -> x |> InventoryRenamedMapper.ofDTO |> Result.map InventoryEvent.InventoryRenamed
    | :? ItemsAddedToInventoryEvent as x ->
        x
        |> ItemsAddedToInventoryMapper.ofDTO
        |> Result.map InventoryEvent.ItemsAddedToInventory
    | :? ItemsRemovedFromInventoryEvent as x ->
        x
        |> ItemsRemovedFromInventoryMapper.ofDTO
        |> Result.map InventoryEvent.ItemsRemovedFromInventory
    | :? ItemInStockEvent as x -> x |> ItemInStockMapper.ofDTO |> Result.map InventoryEvent.ItemInStock
    | :? ItemWentOutOfStockEvent as x ->
        x
        |> ItemWentOutOfStockMapper.ofDTO
        |> Result.map InventoryEvent.ItemWentOutOfStock
    | :? RequestedMoreItemsThanHaveInStockEvent as x ->
        x
        |> RequestedMoreItemsThanHaveInStockMapper.ofDTO
        |> Result.map InventoryEvent.RequestedMoreItemsThanHaveInStock
    | :? InventoryDeactivatedEvent as x ->
        x
        |> InventoryDeactivatedMapper.ofDTO
        |> Result.map InventoryEvent.InventoryDeactivated
    | x -> raise (EventDtoMappingException $"Unknown event DTO type: {x.GetType().FullName}")

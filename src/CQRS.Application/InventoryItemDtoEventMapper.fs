namespace CQRS.Application

open CQRS.Domain.Inventory
open CQRS.Mapping
open CQRS.Ports.EventStore

// Maps any given DTO event from CQRS.DTO.V1 to union type InventoryItemEvent
// Fails immediately with EventMappingException id there are any mapping errors
// Fails immediately with UnknownEventTypeException if DTO event is not supported / missing from mapping cases

type InventoryItemDtoEventMapper() =
    let mapOfFail r =
        match r with
        | Ok x -> x
        | Error e -> raise (EventMappingException(System.Text.Json.JsonSerializer.Serialize(e)))

    let domainEventToDtoEvent =
        function
        | InventoryItemCreated e -> (InventoryItemCreatedMapper.fromDomain e) :> obj
        | InventoryItemRenamed e -> (InventoryItemRenamedMapper.fromDomain e) :> obj
        | ItemsAddedToInventory e -> (ItemsAddedToInventoryMapper.fromDomain e) :> obj
        | ItemsRemovedFromInventory e -> (ItemsRemovedFromInventoryMapper.fromDomain e) :> obj
        | ItemNotInStock e -> (ItemNotInStockMapper.fromDomain e) :> obj
        | ItemWentOutOfStock e -> (ItemWentOutOfStockMapper.fromDomain e) :> obj
        | InventoryItemDeactivated e -> (InventoryItemDeactivatedMapper.fromDomain e) :> obj

    interface IEventMapper<InventoryItemEvent> with
        member this.FromDomainEvent(domainEvent) =
            match domainEvent with
            | :? InventoryItemEvent -> (domainEvent :?> InventoryItemEvent) |> domainEventToDtoEvent
            | _ -> raise (UnknownEventTypeException(domainEvent.GetType().FullName))

        member this.ToDomainEvent(dtoWithMetadata) =
            match dtoWithMetadata.Event with
            | :? CQRS.DTO.V1.InventoryItemCreatedEvent ->
                let domainEvent =
                    (dtoWithMetadata.Event :?> CQRS.DTO.V1.InventoryItemCreatedEvent)
                    |> InventoryItemCreatedMapper.toDomain
                    |> mapOfFail

                (InventoryItemEvent.InventoryItemCreated domainEvent)

            | :? CQRS.DTO.V1.InventoryItemRenamedEvent ->
                let domainEvent =
                    (dtoWithMetadata.Event :?> CQRS.DTO.V1.InventoryItemRenamedEvent)
                    |> InventoryItemRenamedMapper.toDomain
                    |> mapOfFail

                (InventoryItemEvent.InventoryItemRenamed domainEvent)

            | :? CQRS.DTO.V1.ItemsAddedToInventoryEvent ->
                let domainEvent =
                    (dtoWithMetadata.Event :?> CQRS.DTO.V1.ItemsAddedToInventoryEvent)
                    |> ItemsAddedToInventoryMapper.toDomain
                    |> mapOfFail

                (InventoryItemEvent.ItemsAddedToInventory domainEvent)

            | :? CQRS.DTO.V1.ItemsRemovedFromInventoryEvent ->
                let domainEvent =
                    (dtoWithMetadata.Event :?> CQRS.DTO.V1.ItemsRemovedFromInventoryEvent)
                    |> ItemsRemovedFromInventoryMapper.toDomain
                    |> mapOfFail

                (InventoryItemEvent.ItemsRemovedFromInventory domainEvent)

            | :? CQRS.DTO.V1.ItemNotInStockEvent ->
                let domainEvent =
                    (dtoWithMetadata.Event :?> CQRS.DTO.V1.ItemNotInStockEvent)
                    |> ItemNotInStockMapper.toDomain
                    |> mapOfFail

                (InventoryItemEvent.ItemNotInStock domainEvent)

            | :? CQRS.DTO.V1.ItemWentOutOfStockEvent ->
                let domainEvent =
                    (dtoWithMetadata.Event :?> CQRS.DTO.V1.ItemWentOutOfStockEvent)
                    |> ItemWentOutOfStockMapper.toDomain
                    |> mapOfFail

                (InventoryItemEvent.ItemWentOutOfStock domainEvent)

            | :? CQRS.DTO.V1.InventoryItemDeactivatedEvent ->
                let domainEvent =
                    (dtoWithMetadata.Event :?> CQRS.DTO.V1.InventoryItemDeactivatedEvent)
                    |> InventoryItemDeactivatedMapper.toDomain
                    |> mapOfFail

                (InventoryItemEvent.InventoryItemDeactivated domainEvent)

            | _ -> raise (UnknownEventTypeException dtoWithMetadata.EventType.FullName)

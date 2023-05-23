namespace CQRS.Application

open CQRS.Domain.Inventory
open CQRS.Mapping
open CQRS.Ports.EventStore

// Maps any given DTO event from CQRS.DTO.V1 to union type InventoryEvent
// Fails immediately with EventMappingException id there are any mapping errors
// Fails immediately with UnknownEventTypeException if DTO event is not supported / missing from mapping cases

type InventoryEventDtoMapper() =
    let mapOfFail r =
        match r with
        | Ok x -> x
        | Error e -> raise (EventMappingException(System.Text.Json.JsonSerializer.Serialize(e)))

    let domainEventToDtoEvent =
        function
        | InventoryCreated e -> (InventoryCreatedMapper.fromDomain e) :> obj
        | InventoryRenamed e -> (InventoryRenamedMapper.fromDomain e) :> obj
        | ItemsAddedToInventory e -> (ItemsAddedToInventoryMapper.fromDomain e) :> obj
        | ItemsRemovedFromInventory e -> (ItemsRemovedFromInventoryMapper.fromDomain e) :> obj
        | ItemInStock e -> (ItemInStockMapper.fromDomain e) :> obj
        | ItemWentOutOfStock e -> (ItemWentOutOfStockMapper.fromDomain e) :> obj
        | InventoryDeactivated e -> (InventoryDeactivatedMapper.fromDomain e) :> obj
        | RequestedMoreItemsThanHaveInStock e -> (RequestedMoreItemsThanHaveInStockMapper.fromDomain e) :> obj

    interface IEventMapper<InventoryEvent> with
        member this.FromDomainEvent(domainEvent) =
            match domainEvent with
            | :? InventoryEvent -> (domainEvent :?> InventoryEvent) |> domainEventToDtoEvent
            | _ -> raise (UnknownEventTypeException(domainEvent.GetType().FullName))

        member this.ToDomainEvent(dtoWithMetadata) =
            match dtoWithMetadata.Event with
            | :? CQRS.DTO.V1.InventoryCreatedEvent ->
                let domainEvent =
                    (dtoWithMetadata.Event :?> CQRS.DTO.V1.InventoryCreatedEvent)
                    |> InventoryCreatedMapper.toDomain
                    |> mapOfFail

                (InventoryEvent.InventoryCreated domainEvent)

            | :? CQRS.DTO.V1.InventoryRenamedEvent ->
                let domainEvent =
                    (dtoWithMetadata.Event :?> CQRS.DTO.V1.InventoryRenamedEvent)
                    |> InventoryRenamedMapper.toDomain
                    |> mapOfFail

                (InventoryEvent.InventoryRenamed domainEvent)

            | :? CQRS.DTO.V1.ItemsAddedToInventoryEvent ->
                let domainEvent =
                    (dtoWithMetadata.Event :?> CQRS.DTO.V1.ItemsAddedToInventoryEvent)
                    |> ItemsAddedToInventoryMapper.toDomain
                    |> mapOfFail

                (InventoryEvent.ItemsAddedToInventory domainEvent)

            | :? CQRS.DTO.V1.ItemsRemovedFromInventoryEvent ->
                let domainEvent =
                    (dtoWithMetadata.Event :?> CQRS.DTO.V1.ItemsRemovedFromInventoryEvent)
                    |> ItemsRemovedFromInventoryMapper.toDomain
                    |> mapOfFail

                (InventoryEvent.ItemsRemovedFromInventory domainEvent)

            | :? CQRS.DTO.V1.ItemInStockEvent ->
                let domainEvent =
                    (dtoWithMetadata.Event :?> CQRS.DTO.V1.ItemInStockEvent)
                    |> ItemInStockMapper.toDomain
                    |> mapOfFail

                (InventoryEvent.ItemInStock domainEvent)

            | :? CQRS.DTO.V1.ItemWentOutOfStockEvent ->
                let domainEvent =
                    (dtoWithMetadata.Event :?> CQRS.DTO.V1.ItemWentOutOfStockEvent)
                    |> ItemWentOutOfStockMapper.toDomain
                    |> mapOfFail

                (InventoryEvent.ItemWentOutOfStock domainEvent)

            | :? CQRS.DTO.V1.RequestedMoreItemsThanHaveInStockEvent ->
                let domainEvent =
                    (dtoWithMetadata.Event :?> CQRS.DTO.V1.RequestedMoreItemsThanHaveInStockEvent)
                    |> RequestedMoreItemsThanHaveInStockMapper.toDomain
                    |> mapOfFail

                (InventoryEvent.RequestedMoreItemsThanHaveInStock domainEvent)

            | :? CQRS.DTO.V1.InventoryDeactivatedEvent ->
                let domainEvent =
                    (dtoWithMetadata.Event :?> CQRS.DTO.V1.InventoryDeactivatedEvent)
                    |> InventoryDeactivatedMapper.toDomain
                    |> mapOfFail

                (InventoryEvent.InventoryDeactivated domainEvent)

            | _ -> raise (UnknownEventTypeException dtoWithMetadata.EventType.FullName)

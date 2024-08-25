namespace CQRS.Application

open System
open CQRS.DTO
open CQRS.Domain.Inventory
open CQRS.Mapping
open CQRS.Ports.EventStore
open FPrimitive

// Maps any given DTO event from CQRS.DTO.V1 to union type InventoryEvent
// Fails immediately with EventMappingException if there are any mapping errors
// Fails immediately with UnknownEventTypeException if DTO event is not supported / missing from mapping cases

type InventoryEventStreamDtoMapper() =

    let mapOrFail (result: Result<InventoryEvent, ErrorsByTag>) =
        result |> Result.defaultWith (fun e -> raise (EventMappingException $"{e:A}"))

    interface IEventMapper<InventoryEvent> with
        member this.FromDomainEvent(domainEvent) =
            match domainEvent with
            | x when (isNull x) -> raise (ArgumentNullException("Event instance is required", (nameof domainEvent)))
            | :? InventoryEvent as x -> x |> InventoryEvent'.toDTO |> box
            | _ -> raise (UnknownEventTypeException(domainEvent.GetType().FullName))

        member this.ToDomainEvent(dtoEventWithMetadata) =
            match dtoEventWithMetadata.Event with
            | x when (isNull x) -> raise (ArgumentNullException("Event instance is required", (nameof dtoEventWithMetadata)))
            | :? CqrsEventDto as x -> x |> InventoryEvent'.ofDTO |> mapOrFail
            | _ -> raise (UnknownEventTypeException(dtoEventWithMetadata.Event.GetType().FullName))

namespace CQRS.Application

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
            | :? InventoryEvent as x -> x |> InventoryEvent'.toDTO :> obj
            | _ -> raise (UnknownEventTypeException(domainEvent.GetType().FullName))

        member this.ToDomainEvent(dtoWithMetadata) =
            let evt = dtoWithMetadata.Event

            match evt with
            | :? CqrsEventDto as x -> x |> InventoryEvent'.ofDTO |> mapOrFail
            | _ -> raise (UnknownEventTypeException(evt.GetType().FullName))

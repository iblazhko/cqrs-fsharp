#time

#r "CQRS.Application.Host/bin/Debug/net8.0/FPrimitive.dll"
#r "CQRS.Application.Host/bin/Debug/net8.0/FsToolkit.ErrorHandling.dll"
#r "CQRS.Application.Tests/bin/Debug/net8.0/Microsoft.Extensions.Logging.Abstractions.dll"

#r "CQRS.EntityIds/bin/Debug/net8.0/CQRS.EntityIds.dll"

#r "CQRS.Domain/bin/Debug/net8.0/CQRS.Domain.dll"
#r "CQRS.DTO/bin/Debug/net8.0/CQRS.DTO.dll"
#r "CQRS.Mapping/bin/Debug/net8.0/CQRS.Mapping.dll"

#r "CQRS.Configuration/bin/Debug/net8.0/CQRS.Configuration.dll"
#r "CQRS.Application/bin/Debug/net8.0/CQRS.Application.dll"

#r "CQRS.Ports.EventStore/bin/Debug/net8.0/CQRS.Ports.EventStore.dll"
#r "CQRS.Ports.ProjectionStore/bin/Debug/net8.0/CQRS.Ports.ProjectionStore.dll"
#r "CQRS.Ports.MessageBus/bin/Debug/net8.0/CQRS.Ports.MessageBus.dll"

#r "CQRS.Adapters.InMemoryEventStore/bin/Debug/net8.0/CQRS.Adapters.InMemoryEventStore.dll"

open CQRS.EntityIds
open CQRS.DTO.V1
open CQRS.Domain.Inventory
open CQRS.Application
open CQRS.Ports.EventStore

let eventStore =
    new InMemoryEventStore(SystemTextJsonEventSerializer(), None) :> IEventStore

let inventoryId = InventoryId.newId ()
let inventoryIdAsGuid = inventoryId |> InventoryId.value |> EntityId.value

let createInventoryCommand =
    CreateInventoryCommand(InventoryId = inventoryIdAsGuid, Name = "Product001")

let createTask =
    createInventoryCommand |> CommandDtoHandler.handleCommand eventStore

createTask.Wait()

let renameInventoryCommand =
    RenameInventoryCommand(InventoryId = inventoryIdAsGuid, NewName = "Product001-New")

let renameTask =
    renameInventoryCommand |> CommandDtoHandler.handleCommand eventStore

renameTask.Wait()

// let deactivateInventoryCommand =
//     DeactivateInventoryCommand(InventoryId = inventoryIdAsGuid)
//
// let deactivateTask =
//     deactivateInventoryCommand |> CommandDtoHandler.handleCommand eventStore
//
// TODO: this fails in FSI with CommandProcessingException (ValidationFailure (DoesNotExist (InventoryId (EntityId <guid>)))))
//
// deactivateTask.Wait()

let eventStreamDtoMapper =
    InventoryEventStreamDtoMapper() :> IEventMapper<InventoryEvent>

let stateProjection =
    InventoryEventStreamProjection() :> IEventStreamProjection<InventoryEvent, InventoryState>

let eventStreamId = inventoryId |> InventoryEventStreamId.fromInventoryId

let (eventStreamSession: IEventStreamSession<InventoryEvent, InventoryState>) =
    eventStore.Open(eventStreamId, eventStreamDtoMapper).Result

let events = eventStreamSession.GetAllEvents().Result
printfn $"%A{events}"

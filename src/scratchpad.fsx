#time

#r "CQRS.Application.Host/bin/Debug/net7.0/FPrimitive.dll"
#r "CQRS.Application.Host/bin/Debug/net7.0/FsToolkit.ErrorHandling.dll"

#r "CQRS.EntityIds/bin/Debug/net7.0/CQRS.EntityIds.dll"

#r "CQRS.Domain/bin/Debug/net7.0/CQRS.Domain.dll"
#r "CQRS.DTO/bin/Debug/net7.0/CQRS.DTO.dll"
#r "CQRS.Mapping/bin/Debug/net7.0/CQRS.Mapping.dll"

#r "CQRS.Configuration/bin/Debug/net7.0/CQRS.Configuration.dll"
#r "CQRS.Application/bin/Debug/net7.0/CQRS.Application.dll"

#r "CQRS.Ports.EventStore/bin/Debug/net7.0/CQRS.Ports.EventStore.dll"
#r "CQRS.Ports.ProjectionStore/bin/Debug/net7.0/CQRS.Ports.ProjectionStore.dll"
#r "CQRS.Ports.MessageBus/bin/Debug/net7.0/CQRS.Ports.MessageBus.dll"

#r "CQRS.Adapters.InMemoryEventStore/bin/Debug/net7.0/CQRS.Adapters.InMemoryEventStore.dll"

open CQRS.EntityIds
open CQRS.DTO.V1
open CQRS.Domain.Inventory
open CQRS.Application
open CQRS.Adapters
open CQRS.Ports.EventStore

let eventStore = new InMemoryEventStore(SystemTextJsonEventSerializer(), None)

let inventoryId = InventoryId.newId ()

let inventoryIdAsGuid = inventoryId |> InventoryId.value |> EntityId.value

let createInventoryCommand =
    CreateInventoryCommand(InventoryId = inventoryIdAsGuid, Name = "Product001")

InventoryCommandHandlers.handleCreateInventoryCommand eventStore createInventoryCommand

let renameInventoryCommand =
    RenameInventoryCommand(InventoryId = inventoryIdAsGuid, NewName = "Product001-New")

InventoryCommandHandlers.handleRenameInventoryCommand eventStore renameInventoryCommand

let deactivateInventoryCommand =
    DeactivateInventoryCommand(InventoryId = inventoryIdAsGuid)

InventoryCommandHandlers.handleDeactivateInventoryCommand eventStore deactivateInventoryCommand

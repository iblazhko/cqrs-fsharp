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
open CQRS.Adapters.InMemoryEventStore

let inMemoryEventStore =
    InMemoryEventStore<InventoryItemEvent, InventoryItemState>()

let eventStore = inMemoryEventStore.Initialize()

let inventoryItemId = InventoryItemId.newId ()

let inventoryItemIdAsGuid =
    inventoryItemId |> InventoryItemId.value |> EntityId.value

let createInventoryItemCommand =
    CreateInventoryItemCommand(InventoryItemId = inventoryItemIdAsGuid, Name = "Product001")

let createResultOrError =
    InventoryItemCommandHandlers.handleCreateInventoryItemCommand eventStore createInventoryItemCommand
    |> Async.RunSynchronously

let renameInventoryItemCommand =
    RenameInventoryItemCommand(InventoryItemId = inventoryItemIdAsGuid, NewName = "Product001-New")

let renameResultOrError =
    InventoryItemCommandHandlers.handleRenameInventoryItemCommand eventStore renameInventoryItemCommand
    |> Async.RunSynchronously


let deactivateInventoryItemCommand =
    DeactivateInventoryItemCommand(InventoryItemId = inventoryItemIdAsGuid)

let deactivateResultOrError =
    InventoryItemCommandHandlers.handleDeactivateInventoryItemCommand eventStore deactivateInventoryItemCommand
    |> Async.RunSynchronously

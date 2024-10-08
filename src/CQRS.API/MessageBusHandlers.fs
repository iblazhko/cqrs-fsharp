module CQRS.API.MessageBusHandlers

open System
open System.Threading.Tasks
open CQRS.DTO.V1
open CQRS.EntityIds
open CQRS.Ports.Messaging
open FPrimitive
open Serilog

open CQRS.Ports.Messaging.MessageContextBuilder

let createInventory
    (messageBus: IMessageBus)
    (clock: TimeProvider)
    (cmd: CreateInventoryCommand)
    : Task<Result<AcceptedResponse, ErrorsByTag>> =
    task {
        if (cmd.InventoryId = EntityIdRawValue.Empty) then
            cmd.InventoryId <- (EntityId.newId () |> EntityId.value)

        let messageContext = getNewMessageContext (clock.GetUtcNow())
        Log.Logger.Information("Creating inventory {InventoryId}", cmd.InventoryId)
        do! messageBus.SendCommand(Command<CreateInventoryCommand>(Data = cmd, Context = messageContext), None)
        return Ok(cmd.InventoryId |> AcceptedResponse.create messageContext)
    }

let renameInventory
    (messageBus: IMessageBus)
    (clock: TimeProvider)
    (cmd: RenameInventoryCommand)
    : Task<Result<AcceptedResponse, ErrorsByTag>> =
    task {
        let messageContext = getNewMessageContext (clock.GetUtcNow())
        Log.Logger.Information("Renaming inventory {InventoryId} to {InventoryName}", cmd.InventoryId, cmd.NewName)
        do! messageBus.SendCommand(Command<RenameInventoryCommand>(Data = cmd, Context = messageContext), None)
        return Ok(cmd.InventoryId |> AcceptedResponse.create messageContext)
    }

let addItemsToInventory
    (messageBus: IMessageBus)
    (clock: TimeProvider)
    (cmd: AddItemsToInventoryCommand)
    : Task<Result<AcceptedResponse, ErrorsByTag>> =
    task {
        let messageContext = getNewMessageContext (clock.GetUtcNow())
        Log.Logger.Information("Adding {Count} items to inventory {InventoryId}", cmd.Count, cmd.InventoryId)
        do! messageBus.SendCommand(Command<AddItemsToInventoryCommand>(Data = cmd, Context = messageContext), None)
        return Ok(cmd.InventoryId |> AcceptedResponse.create messageContext)
    }

let removeItemsFromInventory
    (messageBus: IMessageBus)
    (clock: TimeProvider)
    (cmd: RemoveItemsFromInventoryCommand)
    : Task<Result<AcceptedResponse, ErrorsByTag>> =
    task {
        let messageContext = getNewMessageContext (clock.GetUtcNow())
        Log.Logger.Information("Removing {Count} items from inventory {InventoryId}", cmd.Count, cmd.InventoryId)
        do! messageBus.SendCommand(Command<RemoveItemsFromInventoryCommand>(Data = cmd, Context = messageContext), None)
        return Ok(cmd.InventoryId |> AcceptedResponse.create messageContext)
    }

let deactivateInventory
    (messageBus: IMessageBus)
    (clock: TimeProvider)
    (cmd: DeactivateInventoryCommand)
    : Task<Result<AcceptedResponse, ErrorsByTag>> =
    task {
        let messageContext = getNewMessageContext (clock.GetUtcNow())
        Log.Logger.Information("Deactivating inventory {InventoryId}", cmd.InventoryId)
        do! messageBus.SendCommand(Command<DeactivateInventoryCommand>(Data = cmd, Context = messageContext), None)
        return Ok(cmd.InventoryId.ToString() |> AcceptedResponse.create messageContext)
    }

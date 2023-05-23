module CQRS.API.HandlersMessageBusAdapter

open System
open System.Threading.Tasks
open CQRS.DTO
open CQRS.DTO.V1
open CQRS.EntityIds
open CQRS.Ports.Messaging
open CQRS.Ports.Messaging.MessageContextBuilder
open FPrimitive
open Serilog

let createInventory
    (messageBus: IMessageBus)
    (clock: unit -> DateTimeOffset)
    (cmd: CreateInventoryCommand)
    : Task<Result<AcceptedResponse, ErrorsByTag>> =
    task {
        if (cmd.InventoryId = EntityIdRawValue.Empty) then
            cmd.InventoryId <- (EntityId.newId () |> EntityId.value)

        let messageContext = getNewMessageContext clock

        // TODO: Use explicit dependency for logging
        Log.Logger.Information("Creating inventory {InventoryId}", cmd.InventoryId)

        do! messageBus.SendCommand(Message<CreateInventoryCommand>(Data = cmd, Context = messageContext))

        return Ok(cmd.InventoryId.ToString() |> AcceptedResponse.fromEntityId messageContext)
    }

let renameInventory
    (messageBus: IMessageBus)
    (clock: unit -> DateTimeOffset)
    (cmd: RenameInventoryCommand)
    : Task<Result<AcceptedResponse, ErrorsByTag>> =
    task {
        let messageContext = getNewMessageContext clock

        // TODO: Use explicit dependency for logging
        Log.Logger.Information("Renaming inventory {InventoryId} to {InventoryName}", cmd.InventoryId, cmd.NewName)

        do! messageBus.SendCommand(Message<RenameInventoryCommand>(Data = cmd, Context = messageContext))

        return Ok(cmd.InventoryId.ToString() |> AcceptedResponse.fromEntityId messageContext)
    }

let addItemsToInventory
    (messageBus: IMessageBus)
    (clock: unit -> DateTimeOffset)
    (cmd: AddItemsToInventoryCommand)
    : Task<Result<AcceptedResponse, ErrorsByTag>> =
    task {
        let messageContext = getNewMessageContext clock

        // TODO: Use explicit dependency for logging
        Log.Logger.Information("Adding {Count} items to inventory {InventoryId}", cmd.Count, cmd.InventoryId)

        do! messageBus.SendCommand(Message<AddItemsToInventoryCommand>(Data = cmd, Context = messageContext))

        return Ok(cmd.InventoryId.ToString() |> AcceptedResponse.fromEntityId messageContext)
    }

let removeItemsFromInventory
    (messageBus: IMessageBus)
    (clock: unit -> DateTimeOffset)
    (cmd: RemoveItemsFromInventoryCommand)
    : Task<Result<AcceptedResponse, ErrorsByTag>> =
    task {
        let messageContext = getNewMessageContext clock

        // TODO: Use explicit dependency for logging
        Log.Logger.Information("Removing {Count} items from inventory {InventoryId}", cmd.Count, cmd.InventoryId)

        do! messageBus.SendCommand(Message<RemoveItemsFromInventoryCommand>(Data = cmd, Context = messageContext))

        return Ok(cmd.InventoryId.ToString() |> AcceptedResponse.fromEntityId messageContext)
    }

let deactivateInventory
    (messageBus: IMessageBus)
    (clock: unit -> DateTimeOffset)
    (cmd: DeactivateInventoryCommand)
    : Task<Result<AcceptedResponse, ErrorsByTag>> =
    task {
        let messageContext = getNewMessageContext clock

        // TODO: Use explicit dependency for logging
        Log.Logger.Information("Deactivating inventory {InventoryId}", cmd.InventoryId)

        do! messageBus.SendCommand(Message<DeactivateInventoryCommand>(Data = cmd, Context = messageContext))

        return Ok(cmd.InventoryId.ToString() |> AcceptedResponse.fromEntityId messageContext)
    }

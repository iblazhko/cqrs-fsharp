module CQRS.API.Handlers

open System
open System.Threading.Tasks
open CQRS.DTO
open CQRS.DTO.V1
open CQRS.EntityIds
open CQRS.Ports.Messaging
open CQRS.Ports.Messaging.MessageContextBuilder
open FPrimitive
open Microsoft.Extensions.Logging

let createInventoryItem
    (messageBus: IMessageBus)
    (clock: unit -> DateTimeOffset)
    (logger: ILogger<CreateInventoryItemCommand>)
    (inventoryItem: CreateInventoryItemCommand)
    : Task<Result<AcceptedResponse, ErrorsByTag>> =
    task {
        if (inventoryItem.InventoryItemId = EntityIdRawValue.Empty) then
            inventoryItem.InventoryItemId <- (EntityId.newId () |> EntityId.value)

        let messageContext = getNewMessageContext clock

        do! messageBus.SendCommand(Message<CreateInventoryItemCommand>(Data = inventoryItem, Context = messageContext))

        logger.LogInformation $"Creating an inventory item: {inventoryItem.InventoryItemId}"

        return
            Ok(
                inventoryItem.InventoryItemId.ToString()
                |> AcceptedResponse.fromEntityId messageContext
            )
    }

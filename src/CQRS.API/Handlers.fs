module CQRS.API.Handlers

open System
open System.Threading.Tasks
open CQRS.DTO
open CQRS.DTO.V1
open CQRS.EntityIds
open CQRS.Ports.Messaging
open CQRS.Ports.Messaging.MessageContextBuilder
open CQRS.Ports.ProjectionStore
open CQRS.Projections
open FPrimitive
open Serilog

type DocumentQueryResult =
    | Document of InventoryItemViewModel
    | NotFound
    | BadRequest of ErrorsByTag

let createInventoryItem
    (messageBus: IMessageBus)
    (clock: unit -> DateTimeOffset)
    (cmd: CreateInventoryItemCommand)
    : Task<Result<AcceptedResponse, ErrorsByTag>> =
    task {
        if (cmd.InventoryItemId = EntityIdRawValue.Empty) then
            cmd.InventoryItemId <- (EntityId.newId () |> EntityId.value)

        let messageContext = getNewMessageContext clock

        do! messageBus.SendCommand(Message<CreateInventoryItemCommand>(Data = cmd, Context = messageContext))

        Log.Logger.Information $"Creating an inventory item: {cmd.InventoryItemId}"

        return Ok(cmd.InventoryItemId.ToString() |> AcceptedResponse.fromEntityId messageContext)
    }

let getInventoryItem
    (projectionsStore: IDocumentStore<InventoryItemViewModel>)
    (inventoryItemId: EntityId)
    : Task<DocumentQueryResult> =
    task {
        Log.Logger.Information $"Retrieving an inventory item: {inventoryItemId}"
        use! collection = projectionsStore.OpenDocumentCollection(InventoryItemsCollection.InventoryItemsProjectionId)

        let documentId = inventoryItemId |> EntityId.toString |> DocumentId.create
        let! document = collection.GetById(documentId)

        let result =
            match document with
            | Some vm -> Document vm
            | None -> NotFound

        return result
    }

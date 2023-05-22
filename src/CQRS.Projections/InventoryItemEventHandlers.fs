module CQRS.Projections.InventoryItemEventHandlers

open CQRS.DTO.V1
open CQRS.Mapping
open CQRS.Ports.ProjectionStore
open CQRS.Projections
open CQRS.Projections.DtoEventHandler
open CQRS.Projections.InventoryItemProjectionApplier

(*
Generic template for event handlers
*)

let private handleInventoryEvent<'TEventDto, 'TEvent, 'TViewModel>
    (context: DomainEventHandlerContext<'TEventDto, 'TEvent, 'TViewModel>)
    (dto: 'TEventDto)
    =
    dto |> DtoEventHandler.handleEvent context

(*
Specific event handlers
*)

let InventoryItemProjectionId _ = "InventoryItems"

let handleInventoryItemCreatedEvent
    (documentStore: IDocumentStore<InventoryItemViewModel>)
    (dto: InventoryItemCreatedEvent)
    =
    dto
    |> handleInventoryEvent
        { DtoMapper = InventoryItemCreatedMapper.toDomain
          DocumentCollectionIdFromEvent = InventoryItemProjectionId
          DocumentIdFromEvent = fun x -> x.InventoryItemId |> InventoryItemProjectionDocumentId.fromInventoryItemId
          DocumentStore = documentStore
          ViewModelUpdateAction = applyInventoryItemCreated }

let handleInventoryItemRenamedEvent
    (documentStore: IDocumentStore<InventoryItemViewModel>)
    (dto: InventoryItemRenamedEvent)
    =
    dto
    |> handleInventoryEvent
        { DtoMapper = InventoryItemRenamedMapper.toDomain
          DocumentCollectionIdFromEvent = InventoryItemProjectionId
          DocumentIdFromEvent = fun x -> x.InventoryItemId |> InventoryItemProjectionDocumentId.fromInventoryItemId
          DocumentStore = documentStore
          ViewModelUpdateAction = applyInventoryItemRenamed }

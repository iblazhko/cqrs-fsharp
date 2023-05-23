module CQRS.Projections.InventoryEventHandlers

open CQRS.DTO.V1
open CQRS.Mapping
open CQRS.Ports.ProjectionStore
open CQRS.Projections
open CQRS.Projections.DtoEventHandler
open CQRS.Projections.InventoryProjectionApplier

(*
Generic template for event handlers
*)

let private handleInventoryEvent<'TEventDto, 'TEvent, 'TViewModel when 'TViewModel: null>
    (context: DomainEventHandlerContext<'TEventDto, 'TEvent, 'TViewModel>)
    (dto: 'TEventDto)
    =
    dto |> DtoEventHandler.handleEvent context

(*
Specific event handlers
*)

let getProjectionId _ =
    InventoryCollection.InventoryProjectionId

let getDocumentId inventoryId =
    inventoryId |> InventoryProjectionDocumentId.fromInventoryId

let handleInventoryCreatedEvent (projectionStore: IProjectionStore<InventoryViewModel>) (dto: InventoryCreatedEvent) =
    dto
    |> handleInventoryEvent
        { DtoMapper = InventoryCreatedMapper.toDomain
          DocumentCollectionIdFromEvent = getProjectionId
          DocumentIdFromEvent = fun x -> x.InventoryId |> getDocumentId
          ProjectionStore = projectionStore
          ViewModelUpdateAction = applyInventoryCreated }

let handleInventoryRenamedEvent (projectionStore: IProjectionStore<InventoryViewModel>) (dto: InventoryRenamedEvent) =
    dto
    |> handleInventoryEvent
        { DtoMapper = InventoryRenamedMapper.toDomain
          DocumentCollectionIdFromEvent = getProjectionId
          DocumentIdFromEvent = fun x -> x.InventoryId |> getDocumentId
          ProjectionStore = projectionStore
          ViewModelUpdateAction = applyInventoryRenamed }

let handleItemsAddedToInventoryEvent
    (projectionStore: IProjectionStore<InventoryViewModel>)
    (dto: ItemsAddedToInventoryEvent)
    =
    dto
    |> handleInventoryEvent
        { DtoMapper = ItemsAddedToInventoryMapper.toDomain
          DocumentCollectionIdFromEvent = getProjectionId
          DocumentIdFromEvent = fun x -> x.InventoryId |> getDocumentId
          ProjectionStore = projectionStore
          ViewModelUpdateAction = applyItemsAddedToInventory }

let handleItemsRemovedFromInventoryEvent
    (projectionStore: IProjectionStore<InventoryViewModel>)
    (dto: ItemsRemovedFromInventoryEvent)
    =
    dto
    |> handleInventoryEvent
        { DtoMapper = ItemsRemovedFromInventoryMapper.toDomain
          DocumentCollectionIdFromEvent = getProjectionId
          DocumentIdFromEvent = fun x -> x.InventoryId |> getDocumentId
          ProjectionStore = projectionStore
          ViewModelUpdateAction = applyItemsRemovedFromInventory }

let handleInventoryDeactivatedEvent
    (projectionStore: IProjectionStore<InventoryViewModel>)
    (dto: InventoryDeactivatedEvent)
    =
    dto
    |> handleInventoryEvent
        { DtoMapper = InventoryDeactivatedMapper.toDomain
          DocumentCollectionIdFromEvent = getProjectionId
          DocumentIdFromEvent = fun x -> x.InventoryId |> getDocumentId
          ProjectionStore = projectionStore
          ViewModelUpdateAction = applyInventoryDeactivated }

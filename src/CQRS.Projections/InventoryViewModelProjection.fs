module CQRS.Projections.InventoryViewModelProjection

open CQRS.DTO
open CQRS.Domain.Inventory
open CQRS.Mapping
open CQRS.Ports.ProjectionStore

let apply (vm: InventoryViewModel) (evt: InventoryEvent) =
    match evt with
    | InventoryCreated x ->
        vm.InventoryId <- x.InventoryId |> InventoryId'.toDTO
        vm.Name <- x.Name |> InventoryName'.toDTO
        vm.IsActive <- x.IsActive
        vm
    | InventoryRenamed x ->
        vm.Name <- x.NewName |> InventoryName'.toDTO
        vm
    | ItemsAddedToInventory x ->
        vm.StockQuantity <- x.NewStockQuantity |> StockQuantity'.toDTO
        vm
    | ItemsRemovedFromInventory x ->
        vm.StockQuantity <- x.NewStockQuantity |> StockQuantity'.toDTO
        vm
    | InventoryDeactivated _ ->
        vm.IsActive <- false
        vm
    | ItemInStock _
    | ItemWentOutOfStock _ -> vm

let private getProjectionId _ =
    InventoryCollection.InventoryProjectionId

let private getDocumentId evt =
    evt
    |> InventoryEvent.inventoryId
    |> InventoryProjectionDocumentId.fromInventoryId

let private updateVm evt vm = evt |> apply vm

let handleInventoryEvent<'TEventDto, 'TViewModel when 'TEventDto :> CqrsEventDto and 'TViewModel: null>
    (projectionStore: IProjectionStore<InventoryViewModel>)
    (dto: 'TEventDto)
    =
    dto
    |> InventoryEventDtoHandler.handleEvent
        { ProjectionStore = projectionStore
          EventFromDto = InventoryEvent'.ofDTO
          DocumentCollectionIdFromEvent = getProjectionId
          DocumentIdFromEvent = getDocumentId
          ViewModelUpdateAction = updateVm }

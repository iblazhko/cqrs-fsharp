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
    | ItemWentOutOfStock _
    | RequestedMoreItemsThanHaveInStock _ -> vm

let private getProjectionId _ =
    InventoryCollection.InventoryProjectionId

let private getDocumentId evt =
    match evt with
    | InventoryCreated x -> x.InventoryId
    | InventoryRenamed x -> x.InventoryId
    | ItemsAddedToInventory x -> x.InventoryId
    | ItemsRemovedFromInventory x -> x.InventoryId
    | ItemInStock x -> x.InventoryId
    | ItemWentOutOfStock x -> x.InventoryId
    | RequestedMoreItemsThanHaveInStock x -> x.InventoryId
    | InventoryDeactivated x -> x.InventoryId
    |> InventoryProjectionDocumentId.fromInventoryId

let private updateVm evt vm = evt |> apply vm

let handleInventoryEvent<'TEventDto, 'TViewModel when 'TEventDto :> CqrsEventDto and 'TViewModel: null>
    (projectionStore: IProjectionStore<InventoryViewModel>)
    (dto: 'TEventDto)
    =
    dto
    |> InventoryEventDtoHandler.handleEvent
        { ProjectionStore = projectionStore
          DocumentCollectionIdFromEvent = getProjectionId
          DocumentIdFromEvent = getDocumentId
          ViewModelUpdateAction = updateVm }

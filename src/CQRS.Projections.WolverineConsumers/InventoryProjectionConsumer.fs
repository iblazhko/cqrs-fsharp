namespace CQRS.Projections.WolverineConsumers

open CQRS.DTO.V1
open CQRS.Ports.ProjectionStore
open CQRS.Projections
open CQRS.Projections.WolverineConsumers

type InventoryProjectionConsumer(projectionStore: IProjectionStore<InventoryViewModel>) =
    member _.HandleAsync(message: InventoryCreatedEvent) =
        WolverineDtoConsumer.handleEvent InventoryViewModelProjection.handleInventoryEvent projectionStore message

    member _.HandleAsync(message: InventoryRenamedEvent) =
        WolverineDtoConsumer.handleEvent InventoryViewModelProjection.handleInventoryEvent projectionStore message

    member _.HandleAsync(message: ItemsAddedToInventoryEvent) =
        WolverineDtoConsumer.handleEvent InventoryViewModelProjection.handleInventoryEvent projectionStore message

    member _.HandleAsync(message: ItemsRemovedFromInventoryEvent) =
        WolverineDtoConsumer.handleEvent InventoryViewModelProjection.handleInventoryEvent projectionStore message

    member _.HandleAsync(message: InventoryDeactivatedEvent) =
        WolverineDtoConsumer.handleEvent InventoryViewModelProjection.handleInventoryEvent projectionStore message

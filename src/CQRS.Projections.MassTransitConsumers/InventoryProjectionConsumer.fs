namespace CQRS.Projections.MassTransitConsumers

open CQRS.DTO.V1
open CQRS.Ports.ProjectionStore
open CQRS.Projections
open CQRS.Projections.MassTransitConsumers
open MassTransit

(*
MassTransit convention is to create a queue *per consumer class*.
For view model projection it does not make much of a difference if we have
separate consumer classes per event type or single consumer class consuming
multiple event types.
Here we use a single consumer for the view model projection.
*)

type InventoryProjectionConsumer(projectionStore: IProjectionStore<InventoryViewModel>) =
    interface IConsumer<InventoryCreatedEvent> with
        member this.Consume(context: ConsumeContext<InventoryCreatedEvent>) =
            context
            |> MassTransitDtoConsumer.handleEvent InventoryViewModelProjection.handleInventoryEvent projectionStore

    interface IConsumer<InventoryRenamedEvent> with
        member this.Consume(context: ConsumeContext<InventoryRenamedEvent>) =
            context
            |> MassTransitDtoConsumer.handleEvent InventoryViewModelProjection.handleInventoryEvent projectionStore

    interface IConsumer<ItemsAddedToInventoryEvent> with
        member this.Consume(context: ConsumeContext<ItemsAddedToInventoryEvent>) =
            context
            |> MassTransitDtoConsumer.handleEvent InventoryViewModelProjection.handleInventoryEvent projectionStore

    interface IConsumer<ItemsRemovedFromInventoryEvent> with
        member this.Consume(context: ConsumeContext<ItemsRemovedFromInventoryEvent>) =
            context
            |> MassTransitDtoConsumer.handleEvent InventoryViewModelProjection.handleInventoryEvent projectionStore

    interface IConsumer<InventoryDeactivatedEvent> with
        member this.Consume(context: ConsumeContext<InventoryDeactivatedEvent>) =
            context
            |> MassTransitDtoConsumer.handleEvent InventoryViewModelProjection.handleInventoryEvent projectionStore

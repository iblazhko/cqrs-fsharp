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

type InventoryItemProjectionConsumer(projectionStore: IProjectionStore<InventoryItemViewModel>) =
    interface IConsumer<InventoryItemCreatedEvent> with
        member this.Consume(context: ConsumeContext<InventoryItemCreatedEvent>) =
            context
            |> EventConsumer.handleMassTransitMessage
                InventoryItemEventHandlers.handleInventoryItemCreatedEvent
                projectionStore

    interface IConsumer<InventoryItemRenamedEvent> with
        member this.Consume(context: ConsumeContext<InventoryItemRenamedEvent>) =
            context
            |> EventConsumer.handleMassTransitMessage
                InventoryItemEventHandlers.handleInventoryItemRenamedEvent
                projectionStore

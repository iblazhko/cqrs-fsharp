namespace CQRS.Projections.MassTransitConsumers

open CQRS.DTO.V1
open CQRS.Ports.ProjectionStore
open CQRS.Projections
open CQRS.Projections.MassTransitConsumers
open MassTransit
open Serilog

(*
Note: MassTransit convention is to create a queue _per consumer class_.
For events it does not make much of a difference if we have
separate consumer classes per event or single consumer class consuming
multiple event types.
*)

type InventoryItemProjectionConsumer(documentStore: IDocumentStore<InventoryItemViewModel>) =
    interface IConsumer<InventoryItemCreatedEvent> with
        member this.Consume(context: ConsumeContext<InventoryItemCreatedEvent>) =
            context
            |> EventConsumer.handleMassTransitMessage
                InventoryItemEventHandlers.handleInventoryItemCreatedEvent
                documentStore

    interface IConsumer<InventoryItemRenamedEvent> with
        member this.Consume(context: ConsumeContext<InventoryItemRenamedEvent>) =
            context
            |> EventConsumer.handleMassTransitMessage
                InventoryItemEventHandlers.handleInventoryItemRenamedEvent
                documentStore

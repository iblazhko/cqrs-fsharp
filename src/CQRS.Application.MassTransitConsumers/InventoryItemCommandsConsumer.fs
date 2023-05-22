namespace CQRS.Application.MassTransitConsumers

open CQRS.Application
open CQRS.DTO.V1
open CQRS.Ports.EventStore
open MassTransit

(*
MassTransit convention is to create a queue *per consumer class*.
For commands, we need separate queue *per command type* to match
our internal sending conventions (see `CQRS.Infrastructure.MassTransitConventions`),
hence need separate consumer class per command with consumer class name matching command name
(`Consumer` suffix is dropped from queue name by MassTransit queue name formatter)
*)

type CreateInventoryItemCommandConsumer(eventStore: IEventStore) =
    interface IConsumer<CreateInventoryItemCommand> with
        member this.Consume(context) =
            context
            |> CommandConsumer.handleMassTransitMessage
                InventoryItemCommandHandlers.handleCreateInventoryItemCommand
                eventStore

type RenameInventoryItemCommandConsumer(eventStore: IEventStore) =
    interface IConsumer<RenameInventoryItemCommand> with
        member this.Consume(context) =
            context
            |> CommandConsumer.handleMassTransitMessage
                InventoryItemCommandHandlers.handleRenameInventoryItemCommand
                eventStore

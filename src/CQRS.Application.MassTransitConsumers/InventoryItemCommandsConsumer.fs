namespace CQRS.Application.MassTransitConsumers

open CQRS.Application
open CQRS.DTO.V1
open CQRS.Ports.EventStore
open MassTransit

(*
Note: MassTransit convention is to create a queue _per consumer class_.
For commands, we need separate queue _per command type_ to match
our internal sending conventions (see CQRS.Infrastructure.MassTransitConventions),
hence need separate consumer class per command with consumer class name matching command name
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

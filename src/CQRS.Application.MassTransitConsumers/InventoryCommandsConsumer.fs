namespace CQRS.Application.MassTransitConsumers

open CQRS.DTO.V1
open CQRS.Ports.EventStore
open MassTransit

(*
MassTransit convention is to create a queue *per consumer class*.
For commands, we need separate queue *per command type* to match
our internal sending conventions (see `CQRS.Infrastructure.MassTransitConventions`),
hence need separate consumer class per command with consumer class name matching command name
(`Consumer` suffix is dropped by MassTransit queue name formatter)
*)

type CreateInventoryCommandConsumer(eventStore: IEventStore) =
    interface IConsumer<CreateInventoryCommand> with
        member this.Consume(context) =
            context |> MassTransitDtoConsumer.handleCommand eventStore

type RenameInventoryCommandConsumer(eventStore: IEventStore) =
    interface IConsumer<RenameInventoryCommand> with
        member this.Consume(context) =
            context |> MassTransitDtoConsumer.handleCommand eventStore

type AddItemsToInventoryCommandConsumer(eventStore: IEventStore) =
    interface IConsumer<AddItemsToInventoryCommand> with
        member this.Consume(context) =
            context |> MassTransitDtoConsumer.handleCommand eventStore

type RemoveItemsFromInventoryCommandConsumer(eventStore: IEventStore) =
    interface IConsumer<RemoveItemsFromInventoryCommand> with
        member this.Consume(context) =
            context |> MassTransitDtoConsumer.handleCommand eventStore

type DeactivateInventoryCommandConsumer(eventStore: IEventStore) =
    interface IConsumer<DeactivateInventoryCommand> with
        member this.Consume(context) =
            context |> MassTransitDtoConsumer.handleCommand eventStore

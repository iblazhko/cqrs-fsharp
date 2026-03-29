namespace CQRS.Application.MassTransitConsumers

open CQRS.Application
open CQRS.DTO.V1
open MassTransit

(*
MassTransit convention is to create a queue *per consumer class*.
For commands, we need separate queue *per command type* to match
our internal sending conventions (see `CQRS.Infrastructure.MassTransitConventions`),
hence need separate consumer class per command with consumer class name matching command name
(`Consumer` suffix is dropped by MassTransit queue name formatter)
*)

type CreateInventoryCommandConsumer(env: ApplicationEnvironment) =
    interface IConsumer<CreateInventoryCommand> with
        member this.Consume(context) =
            context |> MassTransitDtoConsumer.handleCommand env

type RenameInventoryCommandConsumer(env: ApplicationEnvironment) =
    interface IConsumer<RenameInventoryCommand> with
        member this.Consume(context) =
            context |> MassTransitDtoConsumer.handleCommand env

type AddItemsToInventoryCommandConsumer(env: ApplicationEnvironment) =
    interface IConsumer<AddItemsToInventoryCommand> with
        member this.Consume(context) =
            context |> MassTransitDtoConsumer.handleCommand env

type RemoveItemsFromInventoryCommandConsumer(env: ApplicationEnvironment) =
    interface IConsumer<RemoveItemsFromInventoryCommand> with
        member this.Consume(context) =
            context |> MassTransitDtoConsumer.handleCommand env

type DeactivateInventoryCommandConsumer(env: ApplicationEnvironment) =
    interface IConsumer<DeactivateInventoryCommand> with
        member this.Consume(context) =
            context |> MassTransitDtoConsumer.handleCommand env

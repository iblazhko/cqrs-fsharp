namespace CQRS.Application.MassTransitConsumers

open CQRS.Application
open CQRS.DTO.V1
open CQRS.Domain.Moon
open CQRS.Ports.EventStore
open CQRS.Ports.Time
open MassTransit

module SystemLocation =
    let location = "Europe/London"
(*
MassTransit convention is to create a queue *per consumer class*.
For commands, we need separate queue *per command type* to match
our internal sending conventions (see `CQRS.Infrastructure.MassTransitConventions`),
hence need separate consumer class per command with consumer class name matching command name
(`Consumer` suffix is dropped by MassTransit queue name formatter)
*)

type CreateInventoryCommandConsumer(clock: IClock, moonPhase: IMoonPhaseService, eventStore: IEventStore) =
    interface IConsumer<CreateInventoryCommand> with
        member this.Consume(context) =
            context
            |> MassTransitDtoConsumer.handleCommand
                { Location = SystemLocation.location
                  Clock = clock
                  EventStore = eventStore
                  MoonPhase = moonPhase }

type RenameInventoryCommandConsumer(clock: IClock, moonPhase: IMoonPhaseService, eventStore: IEventStore) =
    interface IConsumer<RenameInventoryCommand> with
        member this.Consume(context) =
            context
            |> MassTransitDtoConsumer.handleCommand
                { Location = SystemLocation.location
                  Clock = clock
                  EventStore = eventStore
                  MoonPhase = moonPhase }

type AddItemsToInventoryCommandConsumer(clock: IClock, moonPhase: IMoonPhaseService, eventStore: IEventStore) =
    interface IConsumer<AddItemsToInventoryCommand> with
        member this.Consume(context) =
            context
            |> MassTransitDtoConsumer.handleCommand
                { Location = SystemLocation.location
                  Clock = clock
                  EventStore = eventStore
                  MoonPhase = moonPhase }

type RemoveItemsFromInventoryCommandConsumer(clock: IClock, moonPhase: IMoonPhaseService, eventStore: IEventStore) =
    interface IConsumer<RemoveItemsFromInventoryCommand> with
        member this.Consume(context) =
            context
            |> MassTransitDtoConsumer.handleCommand
                { Location = SystemLocation.location
                  Clock = clock
                  EventStore = eventStore
                  MoonPhase = moonPhase }

type DeactivateInventoryCommandConsumer(clock: IClock, moonPhase: IMoonPhaseService, eventStore: IEventStore) =
    interface IConsumer<DeactivateInventoryCommand> with
        member this.Consume(context) =
            context
            |> MassTransitDtoConsumer.handleCommand
                { Location = SystemLocation.location
                  Clock = clock
                  EventStore = eventStore
                  MoonPhase = moonPhase }

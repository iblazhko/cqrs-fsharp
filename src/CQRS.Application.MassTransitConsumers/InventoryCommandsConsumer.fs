namespace CQRS.Application.MassTransitConsumers

open System
open CQRS.Application
open CQRS.Application.CommandProcessingStatusRecording
open CQRS.DTO.V1
open CQRS.Ports.EventStore
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

type CreateInventoryCommandConsumer
    (
        clock: TimeProvider,
        moonPhase: IMoonPhaseService,
        eventStore: IEventStore,
        commandProcessingStatusRecorder: ICommandProcessingStatusRecordingService
    ) =
    interface IConsumer<CreateInventoryCommand> with
        member this.Consume(context) =
            context
            |> MassTransitDtoConsumer.handleCommand
                { Location = SystemLocation.location
                  Clock = clock
                  EventStore = eventStore
                  MoonPhase = moonPhase
                  CommandProcessingStatusRecorder = commandProcessingStatusRecorder }

type RenameInventoryCommandConsumer
    (
        clock: TimeProvider,
        moonPhase: IMoonPhaseService,
        eventStore: IEventStore,
        commandProcessingStatusRecorder: ICommandProcessingStatusRecordingService
    ) =
    interface IConsumer<RenameInventoryCommand> with
        member this.Consume(context) =
            context
            |> MassTransitDtoConsumer.handleCommand
                { Location = SystemLocation.location
                  Clock = clock
                  EventStore = eventStore
                  MoonPhase = moonPhase
                  CommandProcessingStatusRecorder = commandProcessingStatusRecorder }

type AddItemsToInventoryCommandConsumer
    (
        clock: TimeProvider,
        moonPhase: IMoonPhaseService,
        eventStore: IEventStore,
        commandProcessingStatusRecorder: ICommandProcessingStatusRecordingService
    ) =
    interface IConsumer<AddItemsToInventoryCommand> with
        member this.Consume(context) =
            context
            |> MassTransitDtoConsumer.handleCommand
                { Location = SystemLocation.location
                  Clock = clock
                  EventStore = eventStore
                  MoonPhase = moonPhase
                  CommandProcessingStatusRecorder = commandProcessingStatusRecorder }

type RemoveItemsFromInventoryCommandConsumer
    (
        clock: TimeProvider,
        moonPhase: IMoonPhaseService,
        eventStore: IEventStore,
        commandProcessingStatusRecorder: ICommandProcessingStatusRecordingService
    ) =
    interface IConsumer<RemoveItemsFromInventoryCommand> with
        member this.Consume(context) =
            context
            |> MassTransitDtoConsumer.handleCommand
                { Location = SystemLocation.location
                  Clock = clock
                  EventStore = eventStore
                  MoonPhase = moonPhase
                  CommandProcessingStatusRecorder = commandProcessingStatusRecorder }

type DeactivateInventoryCommandConsumer
    (
        clock: TimeProvider,
        moonPhase: IMoonPhaseService,
        eventStore: IEventStore,
        commandProcessingStatusRecorder: ICommandProcessingStatusRecordingService
    ) =
    interface IConsumer<DeactivateInventoryCommand> with
        member this.Consume(context) =
            context
            |> MassTransitDtoConsumer.handleCommand
                { Location = SystemLocation.location
                  Clock = clock
                  EventStore = eventStore
                  MoonPhase = moonPhase
                  CommandProcessingStatusRecorder = commandProcessingStatusRecorder }

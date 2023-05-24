module CQRS.Application.MassTransitConsumers.MassTransitDtoConsumer

open System.Threading.Tasks
open CQRS.Application
open CQRS.DTO
open CQRS.Ports.EventStore
open MassTransit
open Serilog

let handleCommand<'T when 'T :> CqrsCommandDto and 'T: not struct>
    (eventStore: IEventStore)
    (context: ConsumeContext<'T>)
    : Task =
    task {
        let message = context.Message

        // TODO: Use explicit dependency for logging
        Log.Logger.Information("[MESSAGE-BUS] {MessageType} {@Message}", message.GetType().FullName, message)

        do! message |> CommandDtoHandler.handleCommand eventStore
    }

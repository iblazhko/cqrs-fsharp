module CQRS.Application.MassTransitConsumers.CommandConsumer

open System.Threading.Tasks
open CQRS.DTO
open CQRS.Ports.EventStore
open MassTransit
open Serilog

let handleMassTransitMessage<'T when 'T :> CqrsCommandDto and 'T: not struct>
    (consumeAction: IEventStore -> 'T -> Task)
    (eventStore: IEventStore)
    (context: ConsumeContext<'T>)
    : Task =
    task {
        let message = context.Message
        Log.Logger.Information("[MESSAGE-BUS] {@MessageType} {@Message}", message.GetType(), message)
        do! message |> consumeAction eventStore
    }

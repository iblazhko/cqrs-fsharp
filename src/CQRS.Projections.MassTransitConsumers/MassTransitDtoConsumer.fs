module CQRS.Projections.MassTransitConsumers.MassTransitDtoConsumer

open System.Threading.Tasks
open CQRS.Ports.ProjectionStore
open CQRS.DTO
open MassTransit
open Serilog

let handleEvent<'T, 'TViewModel when 'T :> CqrsEventDto and 'T: not struct and 'TViewModel: null>
    (consumeAction: IProjectionStore<'TViewModel> -> 'T -> Task)
    (projectionStore: IProjectionStore<'TViewModel>)
    (context: ConsumeContext<'T>)
    : Task =
    task {
        let message = context.Message
        Log.Logger.Information("[MESSAGE-BUS] {MessageType} {@Message}", message.GetType().FullName, message)
        do! message |> consumeAction projectionStore
    }

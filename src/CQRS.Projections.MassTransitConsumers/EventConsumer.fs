module CQRS.Projections.MassTransitConsumers.EventConsumer

open System.Threading.Tasks
open CQRS.Ports.ProjectionStore
open CQRS.DTO
open MassTransit
open Serilog

let handleMassTransitMessage<'T, 'TViewModel when 'T :> CqrsEventDto and 'T: not struct>
    (consumeAction: IDocumentStore<'TViewModel> -> 'T -> Task)
    (documentStore: IDocumentStore<'TViewModel>)
    (context: ConsumeContext<'T>)
    : Task =
    task {
        let message = context.Message
        Log.Logger.Information("[RECEIVED] {@MessageType} {@Message}", message.GetType(), message)
        do! message |> consumeAction documentStore
        return ()
    }

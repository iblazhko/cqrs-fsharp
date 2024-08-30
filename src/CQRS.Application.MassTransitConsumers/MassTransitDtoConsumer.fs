module CQRS.Application.MassTransitConsumers.MassTransitDtoConsumer

open System
open System.Text.Json
open System.Threading.Tasks
open CQRS.Application
open CQRS.Application.CommandProcessingStatusRecording
open CQRS.DTO
open MassTransit
open Serilog

let private serializeOptions = JsonSerializerOptions(WriteIndented = false)

let handleCommand<'T when 'T :> CqrsCommandDto and 'T: not struct>
    (env: ApplicationEnvironment)
    (context: ConsumeContext<'T>)
    : Task =
    task {
        let message = context.Message

        // TODO: Use explicit dependency for logging
        Log.Logger.Information("[MESSAGE-BUS] {MessageType} {@Message}", message.GetType().FullName, message)

        let request = CommandProcessingRequest()
        request.CommandId <- if context.MessageId.HasValue then context.MessageId.Value else Guid.Empty
        request.CorrelationId <- if context.CorrelationId.HasValue then context.CorrelationId.Value else Guid.Empty
        request.CausationId <- if context.ConversationId.HasValue then context.RequestId.Value else Guid.Empty
        request.CommandType <- message.GetType().FullName
        request.CommandBody <- JsonSerializer.Serialize(message, serializeOptions)
        request.RequestedAt <- env.Clock.GetUtcNow()

        do! env.CommandProcessingStatusRecorder.RecordCommandProcessingStarted request

        try
            do! message |> InventoryCommandDtoHandler.handleCommand env
            do! env.CommandProcessingStatusRecorder.RecordCommandProcessingCompleted(request.CommandId, env.Clock.GetUtcNow())
        with
        | CommandProcessingException(cpe) -> env.CommandProcessingStatusRecorder.RecordCommandProcessingRejected(request.CommandId, env.Clock.GetUtcNow(), $"%A{cpe}") |> ignore
        | x -> env.CommandProcessingStatusRecorder.RecordCommandProcessingFailed(request.CommandId, env.Clock.GetUtcNow(), $"Internal error: {x.GetType()} {x.Message}") |> ignore
    }

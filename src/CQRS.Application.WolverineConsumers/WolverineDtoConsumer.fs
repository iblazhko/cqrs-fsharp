module CQRS.Application.WolverineConsumers.WolverineDtoConsumer

open System
open System.Text.Json
open System.Threading.Tasks
open CQRS.Application
open CQRS.Application.CommandProcessingStatusRecording
open CQRS.DTO
open Wolverine
open Serilog

let private serializeOptions = JsonSerializerOptions(WriteIndented = false)

let handleCommand<'T when 'T :> CqrsCommandDto and 'T: not struct>
    (env: ApplicationEnvironment)
    (message: 'T)
    (envelope: Envelope)
    : Task =
    task {
        Log.Logger.Information("[MESSAGE-BUS] {MessageType} {@Message}", message.GetType().FullName, message)

        let request = CommandProcessingRequest()
        request.CommandId <- envelope.Id
        request.CorrelationId <-
            if envelope.CorrelationId <> null then
                match Guid.TryParse(envelope.CorrelationId : string) with
                | true, g -> g
                | _ -> Guid.NewGuid()
            else
                Guid.NewGuid()
        request.CausationId <-
            if envelope.ParentId <> null then
                match Guid.TryParse(envelope.ParentId : string) with
                | true, g -> g
                | _ -> Guid.Empty
            else
                Guid.Empty
        request.CommandType <- message.GetType().FullName
        request.CommandBody <- JsonSerializer.Serialize(message, serializeOptions)
        request.RequestedAt <- env.Clock.GetUtcNow()

        do! env.CommandProcessingStatusRecorder.RecordCommandProcessingStarted request

        try
            let! result = message |> InventoryCommandDtoHandler.handleCommand env

            match result with
            | Ok() ->
                do! env.CommandProcessingStatusRecorder.RecordCommandProcessingCompleted(request.CommandId, env.Clock.GetUtcNow())
            | Error(CommandDtoMappingError e) ->
                do! env.CommandProcessingStatusRecorder.RecordCommandProcessingRejected(request.CommandId, env.Clock.GetUtcNow(), $"%A{e}")
            | Error(CommandProcessingError f) ->
                do! env.CommandProcessingStatusRecorder.RecordCommandProcessingRejected(request.CommandId, env.Clock.GetUtcNow(), $"%A{f}")
        with x ->
            do! env.CommandProcessingStatusRecorder.RecordCommandProcessingFailed(request.CommandId, env.Clock.GetUtcNow(), $"Internal error: {x.GetType()} {x.Message}")
    }

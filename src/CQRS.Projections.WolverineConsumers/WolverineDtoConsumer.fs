module CQRS.Projections.WolverineConsumers.WolverineDtoConsumer

open System.Threading.Tasks
open CQRS.Ports.ProjectionStore
open CQRS.DTO
open Serilog

let handleEvent<'T, 'TViewModel when 'T :> CqrsEventDto and 'T: not struct and 'TViewModel: null>
    (consumeAction: IProjectionStore<'TViewModel> -> 'T -> Task)
    (projectionStore: IProjectionStore<'TViewModel>)
    (message: 'T)
    : Task =
    task {
        Log.Logger.Information("[MESSAGE-BUS] {MessageType} {@Message}", message.GetType().FullName, message)
        do! message |> consumeAction projectionStore
    }

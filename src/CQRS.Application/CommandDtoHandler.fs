namespace CQRS.Application

open System.Threading.Tasks
open CQRS.DTO
open CQRS.Domain
open CQRS.Domain.Inventory
open CQRS.Mapping
open FPrimitive
open FsToolkit.ErrorHandling

exception CommandDtoMappingException of ErrorsByTag
exception CommandProcessingException of InventoryFailure

module CommandDtoHandler =
    let eventDtoMapper = InventoryEventStreamDtoMapper()
    let stateProjection = InventoryEventStreamProjection()

    let private streamIdFromCommand (cmd: InventoryCommand) =
        match cmd with
        | CreateInventory x -> x.InventoryId
        | RenameInventory x -> x.InventoryId
        | AddItemsToInventory x -> x.InventoryId
        | RemoveItemsFromInventory x -> x.InventoryId
        | DeactivateInventory x -> x.InventoryId
        |> InventoryEventStreamId.fromInventoryId

    let private handleCreate currentState cmd =
        task { return cmd |> InventoryAggregate.create currentState }

    let private handleRename currentState cmd =
        task { return cmd |> InventoryAggregate.rename currentState }

    let private handleAddItems currentState cmd =
        task { return cmd |> InventoryAggregate.addItems currentState }

    let private handleRemoveItems currentState cmd =
        task { return cmd |> InventoryAggregate.removeItems currentState }

    let private handleDeactivate currentState env cmd =
        task {
            let time = env.Clock.Now()
            let! moonPhase = env.MoonPhase.GetMoonPhase(env.Location, time)
            return cmd |> InventoryAggregate.deactivate currentState moonPhase
        }

    let handleCommand<'TCommandDto when 'TCommandDto :> CqrsCommandDto>
        (env: ApplicationEnvironment)
        (dto: 'TCommandDto)
        : Task = // TODO: Use Result<Task,CommandHandlerFault>
        task {
            let command =
                dto
                |> InventoryCommand'.ofDTO
                |> Result.defaultWith (fun e -> raise (CommandDtoMappingException e))
            // TODO: map error to CommandHandlerFault

            let streamId = command |> streamIdFromCommand
            use! streamSession = env.EventStore.Open(streamId, eventDtoMapper)

            let! currentState = streamSession.GetState(stateProjection)

            let! newEventsResult =
                match command with
                | CreateInventory x -> x |> handleCreate currentState
                | RenameInventory x -> x |> handleRename currentState
                | AddItemsToInventory x -> x |> handleAddItems currentState
                | RemoveItemsFromInventory x -> x |> handleRemoveItems currentState
                | DeactivateInventory x -> x |> handleDeactivate currentState env

            let newEvents =
                newEventsResult
                |> Result.defaultWith (fun x -> raise (CommandProcessingException x))

            do! streamSession.AppendEvents(newEvents |> Seq.map box)
            do! env.EventStore.Save(streamSession)
        // TODO: map EventStore errors to CommandHandlerFault
        }

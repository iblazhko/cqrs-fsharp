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

    let streamIdFromCommand (cmd: InventoryCommand) =
        match cmd with
        | CreateInventory x -> x.InventoryId
        | RenameInventory x -> x.InventoryId
        | AddItemsToInventory x -> x.InventoryId
        | RemoveItemsFromInventory x -> x.InventoryId
        | DeactivateInventory x -> x.InventoryId
        |> InventoryEventStreamId.fromInventoryId

    let handleCommand<'TCommandDto when 'TCommandDto :> CqrsCommandDto>
        (env: ApplicationEnvironment)
        (dto: 'TCommandDto)
        : Task = // TODO: Use Result<Task,CommandHandlerFault>
        task {
            let command =
                dto
                |> InventoryCommandMapper.toDomain
                |> Result.defaultWith (fun e -> raise (CommandDtoMappingException e))
            // TODO: map error to CommandHandlerFault

            // TODO: Move this code into DeactivateInventory branch
            let time = env.Clock.Now()
            let! moonPhase = env.MoonPhase.GetMoonPhase(env.Location, time)

            let streamId = command |> streamIdFromCommand
            use! streamSession = env.EventStore.Open(streamId, eventDtoMapper)

            let! currentState = streamSession.GetState(stateProjection)

            let newEvents =
                match command with
                | CreateInventory x -> x |> InventoryAggregate.create currentState
                | RenameInventory x -> x |> InventoryAggregate.rename currentState
                | AddItemsToInventory x -> x |> InventoryAggregate.addItems currentState
                | RemoveItemsFromInventory x -> x |> InventoryAggregate.removeItems currentState
                | DeactivateInventory x -> x |> InventoryAggregate.deactivate currentState moonPhase
                |> Result.defaultWith (fun x -> raise (CommandProcessingException x))

            do! streamSession.AppendEvents(newEvents |> Seq.map box)
            do! env.EventStore.Save(streamSession)
        // TODO: map EventStore errors to CommandHandlerFault
        }

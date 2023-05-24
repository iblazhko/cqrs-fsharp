namespace CQRS.Application

open System.Threading.Tasks
open CQRS.DTO
open CQRS.Domain
open CQRS.Domain.Inventory
open CQRS.Mapping
open CQRS.Ports.EventStore
open FPrimitive
open FsToolkit.ErrorHandling

exception CommandDtoMappingException of ErrorsByTag
exception CommandProcessingException of InventoryFailure

module CommandDtoHandler =
    let eventDtoMapper = InventoryEventStreamDtoMapper()
    let stateProjection = EventStoreInventorySateProjection()

    let streamIdFromCommand (cmd: InventoryCommand) =
        match cmd with
        | CreateInventory x -> x.InventoryId
        | RenameInventory x -> x.InventoryId
        | AddItemsToInventory x -> x.InventoryId
        | RemoveItemsFromInventory x -> x.InventoryId
        | DeactivateInventory x -> x.InventoryId
        |> InventoryEventStreamId.fromInventoryId

    let handleCommand<'TCommandDto when 'TCommandDto :> CqrsCommandDto>
        (eventStore: IEventStore)
        (dto: 'TCommandDto)
        : Task = // TODO: Use Result<Task,CommandHandlerFault>
        task {
            let command =
                dto
                |> InventoryCommandMapper.toDomain
                |> Result.defaultWith (fun e -> raise (CommandDtoMappingException e))
            // TODO: map error to CommandHandlerFault

            let streamId = command |> streamIdFromCommand
            use! streamSession = eventStore.Open(streamId, eventDtoMapper)

            let! currentState = streamSession.GetState(stateProjection)

            let newEvents =
                command
                |> InventoryAggregate.handle currentState
                |> Result.defaultWith (fun x -> raise (CommandProcessingException x))

            do! streamSession.AppendEvents(newEvents |> Seq.map box)
            do! eventStore.Save(streamSession)
        // TODO: map EventStore errors to CommandHandlerFault
        }

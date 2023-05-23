module CQRS.Application.InventoryCommandHandlers

open CQRS.Application
open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping
open CQRS.Ports.EventStore
open FPrimitive

(*
Generic template for command handlers
*)

let eventDtoMapper = InventoryEventDtoMapper()
let stateProjection = InventorySateProjection()

type private InventoryCommandHandlerContext<'TCommandDto, 'TCommand> =
    { CommandDtoMapper: 'TCommandDto -> Result<'TCommand, ErrorsByTag>
      EventDtoMapper: IEventMapper<InventoryEvent>
      AggregateAction: InventoryState -> 'TCommand -> Result<InventoryEvent seq, ErrorsByTag>
      StreamIdFromCommand: 'TCommand -> EventStreamId
      EventStore: IEventStore
      StateProjection: IEventStreamProjection<InventoryEvent, InventoryState> }

let private handleInventoryCommand<'TCommandDto, 'TCommand>
    (context: InventoryCommandHandlerContext<'TCommandDto, 'TCommand>)
    (dto: 'TCommandDto)
    =
    dto
    |> DtoCommandHandler.handleCommand
        { CommandDtoMapper = context.CommandDtoMapper
          EventDtoMapper = context.EventDtoMapper
          AggregateAction = context.AggregateAction
          StreamIdFromCommand = context.StreamIdFromCommand
          EventStore = context.EventStore
          StateProjection = context.StateProjection }

(*
Specific command handlers
*)

let handleCreateInventoryCommand (eventStore: IEventStore) (dto: CreateInventoryCommand) =
    dto
    |> handleInventoryCommand
        { CommandDtoMapper = CreateInventoryMapper.toDomain
          EventDtoMapper = eventDtoMapper
          AggregateAction = InventoryDomainAggregateAdapter.create
          StreamIdFromCommand = (fun x -> x.InventoryId |> InventoryEventStreamId.fromInventoryId)
          EventStore = eventStore
          StateProjection = stateProjection }

let handleRenameInventoryCommand (eventStore: IEventStore) (dto: RenameInventoryCommand) =
    dto
    |> handleInventoryCommand
        { CommandDtoMapper = RenameInventoryMapper.toDomain
          EventDtoMapper = eventDtoMapper
          AggregateAction = InventoryDomainAggregateAdapter.rename
          StreamIdFromCommand = fun x -> x.InventoryId |> InventoryEventStreamId.fromInventoryId
          EventStore = eventStore
          StateProjection = stateProjection }

let handleAddItemsToInventoryCommand (eventStore: IEventStore) (dto: AddItemsToInventoryCommand) =
    dto
    |> handleInventoryCommand
        { CommandDtoMapper = AddItemsToInventoryMapper.toDomain
          EventDtoMapper = eventDtoMapper
          AggregateAction = InventoryDomainAggregateAdapter.addItemsToInventory
          StreamIdFromCommand = fun x -> x.InventoryId |> InventoryEventStreamId.fromInventoryId
          EventStore = eventStore
          StateProjection = stateProjection }

let handleRemoveItemsFromInventoryCommand (eventStore: IEventStore) (dto: RemoveItemsFromInventoryCommand) =
    dto
    |> handleInventoryCommand
        { CommandDtoMapper = RemoveItemsFromInventoryMapper.toDomain
          EventDtoMapper = eventDtoMapper
          AggregateAction = InventoryDomainAggregateAdapter.removeItemsFromInventory
          StreamIdFromCommand = fun x -> x.InventoryId |> InventoryEventStreamId.fromInventoryId
          EventStore = eventStore
          StateProjection = stateProjection }

let handleDeactivateInventoryCommand (eventStore: IEventStore) (dto: DeactivateInventoryCommand) =
    dto
    |> handleInventoryCommand
        { CommandDtoMapper = DeactivateInventoryMapper.toDomain
          EventDtoMapper = eventDtoMapper
          AggregateAction = InventoryDomainAggregateAdapter.deactivate
          StreamIdFromCommand = fun x -> x.InventoryId |> InventoryEventStreamId.fromInventoryId
          EventStore = eventStore
          StateProjection = stateProjection }

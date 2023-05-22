module CQRS.Application.InventoryItemCommandHandlers

open CQRS.Application
open CQRS.Application.InventoryItemEventStoreProjection
open CQRS.Domain.Inventory
open CQRS.DTO.V1
open CQRS.Mapping
open CQRS.Ports.EventStore
open FPrimitive

(*
Generic template for command handlers
*)

let eventDtoMapper = InventoryItemDtoEventMapper()
let stateProjection = InventoryItemEventStreamProjection()

type private InventoryCommandHandlerContext<'TCommandDto, 'TCommand> =
    { CommandDtoMapper: 'TCommandDto -> Result<'TCommand, ErrorsByTag>
      EventDtoMapper: IEventMapper<InventoryItemEvent>
      AggregateAction: InventoryItemState -> 'TCommand -> Result<InventoryItemEvent seq, ErrorsByTag>
      StreamIdFromCommand: 'TCommand -> EventStreamId
      EventStore: IEventStore
      StateProjection: IEventStreamProjection<InventoryItemEvent, InventoryItemState> }

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

let handleCreateInventoryItemCommand (eventStore: IEventStore) (dto: CreateInventoryItemCommand) =
    dto
    |> handleInventoryCommand
        { CommandDtoMapper = CreateInventoryItemMapper.toDomain
          EventDtoMapper = eventDtoMapper
          AggregateAction = InventoryItemDomainAggregateAdapter.create
          StreamIdFromCommand = (fun x -> x.InventoryItemId |> InventoryItemEventStreamId.fromInventoryItemId)
          EventStore = eventStore
          StateProjection = stateProjection }

let handleRenameInventoryItemCommand (eventStore: IEventStore) (dto: RenameInventoryItemCommand) =
    dto
    |> handleInventoryCommand
        { CommandDtoMapper = RenameInventoryItemMapper.toDomain
          EventDtoMapper = eventDtoMapper
          AggregateAction = InventoryItemDomainAggregateAdapter.rename
          StreamIdFromCommand = fun x -> x.InventoryItemId |> InventoryItemEventStreamId.fromInventoryItemId
          EventStore = eventStore
          StateProjection = stateProjection }

let handleDeactivateInventoryItemCommand (eventStore: IEventStore) (dto: DeactivateInventoryItemCommand) =
    dto
    |> handleInventoryCommand
        { CommandDtoMapper = DeactivateInventoryItemMapper.toDomain
          EventDtoMapper = eventDtoMapper
          AggregateAction = InventoryItemDomainAggregateAdapter.deactivate
          StreamIdFromCommand = fun x -> x.InventoryItemId |> InventoryItemEventStreamId.fromInventoryItemId
          EventStore = eventStore
          StateProjection = stateProjection }

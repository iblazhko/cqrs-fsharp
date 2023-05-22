namespace CQRS.Application

open System.Threading.Tasks
open CQRS.Ports.EventStore
open FPrimitive
open FsToolkit.ErrorHandling

type internal DomainCommandHandlerContext<'TCommandDto, 'TCommand, 'TState, 'TEvent> =
    { CommandDtoMapper: 'TCommandDto -> Result<'TCommand, ErrorsByTag>
      EventDtoMapper: IEventMapper<'TEvent>
      AggregateAction: 'TState -> 'TCommand -> Result<'TEvent seq, ErrorsByTag>
      StreamIdFromCommand: 'TCommand -> EventStreamId
      EventStore: IEventStore
      StateProjection: IEventStreamProjection<'TEvent, 'TState> }

exception CommandHandlerException of ErrorsByTag

module internal DtoCommandHandler =
    let handleCommand<'TCommandDto, 'TCommand, 'TState, 'TEvent>
        (context: DomainCommandHandlerContext<'TCommandDto, 'TCommand, 'TState, 'TEvent>)
        (dto: 'TCommandDto)
        : Task = // TODO: Use Result<Task,CommandHandlerFault>
        task {
            let command =
                dto
                |> context.CommandDtoMapper
                |> Result.defaultWith (fun e -> raise (CommandHandlerException e))
            // TODO: map error to CommandHandlerFault

            let streamId = context.StreamIdFromCommand command
            use! streamSession = context.EventStore.Open(streamId, context.EventDtoMapper)

            let! currentState = streamSession.GetState(context.StateProjection)

            let newEvents =
                context.AggregateAction currentState command
                |> Result.defaultWith (fun e -> raise (CommandHandlerException e))
            // TODO: map error to CommandHandlerFault

            do! streamSession.AppendEvents(newEvents |> Seq.map box)
            do! context.EventStore.Save(streamSession)
        // TODO: map EventStore errors to CommandHandlerFault
        }

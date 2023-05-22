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
        : Task =
        task {
            let command =
                dto
                |> context.CommandDtoMapper
                |> Result.defaultWith (fun e -> raise (CommandHandlerException e))

            let streamId = context.StreamIdFromCommand command
            let streamSession = context.EventStore.Open(streamId, context.EventDtoMapper).Result

            let currentState = streamSession.GetState(context.StateProjection).Result

            let newEvents =
                context.AggregateAction currentState command
                |> Result.defaultWith (fun e -> raise (CommandHandlerException e))

            do! streamSession.AppendEvents(newEvents |> Seq.map box)
            do! context.EventStore.Save(streamSession)

            return ()
        }

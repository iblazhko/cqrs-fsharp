namespace CQRS.Ports.EventStore

open System
open System.Threading.Tasks

(*
These interfaces are meant for interoperability with actual event store
implementations, so we are using interfaces and Tasks here, and do not
use advanced type system (e.g. discriminated unions etc.);
also we represent parameters as tuples instead of using currying.
*)

type EventId = Guid

module EventId =
    let newId () = EventId.NewGuid()

type EventStreamId = string
exception InvalidEventStreamIdException

module EventStreamId =
    let create (s: string) =
        if String.IsNullOrWhiteSpace(s) then
            raise InvalidEventStreamIdException

        EventStreamId(s.ToUpperInvariant())

    let value (s: EventStreamId) = s


type EventStreamVersion = int64

module EventStreamVersion =
    [<Literal>]
    let New: EventStreamVersion = 0L

    let increment v : EventStreamVersion = v + 1L

type EventMetadata =
    { EventId: EventId
      CorrelationId: EventId
      CausationId: EventId }

// This type is intended to be used by IEventStore implementations
type SerializedEventWithMetadata =
    { Event: string
      EventTypeName: string
      Metadata: EventMetadata option }

// This type is intended to be used by IEventStore implementations
type SerializedEventStream =
    { StreamId: EventStreamId
      StreamVersion: EventStreamVersion
      Events: SerializedEventWithMetadata seq }

type EventWithMetadata =
    { Event: obj
      EventType: Type
      Metadata: EventMetadata option }

type EventStream =
    { StreamId: EventStreamId
      StreamVersion: EventStreamVersion
      Events: EventWithMetadata seq }

[<Interface>]
type IEventSerializer =
    abstract member Serialize: (obj * Type) -> string
    abstract member Deserialize: (string * Type) -> obj

// 'TEvent is supposed to be a discriminated union in F# or OneOf<E1,E2,...> in C#
[<Interface>]
type IEventMapper<'TEvent> =
    abstract member ToDomainEvent: EventWithMetadata -> 'TEvent
    abstract member FromDomainEvent: obj -> obj

[<Interface>]
type IEventPublisher =
    abstract member Publish: EventWithMetadata seq -> Task

// 'TEvent is supposed to be a discriminated union in F# or OneOf<E1,E2,...> in C#
[<Interface>]
type IEventStreamProjection<'TEvent, 'TState> =
    abstract member GetInitialState: EventStreamId -> 'TState
    abstract member Apply: 'TState * 'TEvent -> 'TState

[<Interface>]
type IEventStreamSession<'TEvent, 'TState> =
    inherit IDisposable
    inherit IAsyncDisposable
    abstract member GetAllEvents: unit -> Task<EventStream>
    abstract member GetNewEvents: unit -> Task<EventStream>
    abstract member GetState: IEventStreamProjection<'TEvent, 'TState> -> Task<'TState>
    abstract member AppendEvents: obj seq -> Task
    abstract member AppendEventsWithMetadata: EventWithMetadata seq -> Task
    abstract member Lock: unit -> unit

[<Interface>]
type IEventStore =
    inherit IDisposable
    inherit IAsyncDisposable

    abstract member Open<'TEvent, 'TState> :
        EventStreamId * IEventMapper<'TEvent> -> Task<IEventStreamSession<'TEvent, 'TState>>

    abstract member Save: IEventStreamSession<'TEvent, 'TState> -> Task
    abstract member Delete: EventStreamId -> Task<bool>
    abstract member Contains: EventStreamId -> Task<bool>

exception UnknownEventTypeException of string
exception EventMappingException of string
exception ConcurrencyException of string
exception SessionIsLockedException of EventStreamId

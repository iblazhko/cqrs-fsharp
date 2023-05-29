namespace CQRS.Ports.Messaging

open System

type MessagingId = Guid

module MessagingId =
    let newId () = MessagingId.NewGuid()

type Context() =
    member val MessageId: MessagingId = MessagingId.Empty with get, set
    member val CorrelationId: MessagingId = MessagingId.Empty with get, set
    member val CausationId: MessagingId = MessagingId.Empty with get, set
    member val Timestamp: DateTimeOffset = DateTimeOffset.MinValue with get, set

type Message<'T>() =
    member val Context: Context = Context() with get, set
    member val Data: 'T = Unchecked.defaultof<'T> with get, set

type Command<'T>() =
    inherit Message<'T>()

type Event<'T>() =
    inherit Message<'T>()

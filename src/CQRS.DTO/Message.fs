namespace CQRS.DTO

open System

// DTOs are meant to be interoperable with outside world hence using classes with properties

type MessagingId = Guid

module MessagingId =
    let newId () = MessagingId.NewGuid()

type Context() =
    member val MessageId: MessagingId = MessagingId.Empty with get, set
    member val CorrelationId: MessagingId = MessagingId.Empty with get, set
    member val CausationId: MessagingId = MessagingId.Empty with get, set
    member val Timestamp: DateTimeOffset = DateTimeOffset.MinValue with get, set
    override this.ToString() = Json.serialize this

type Message<'T when 'T :> CqrsDto>() =
    member val Context: Context = Context() with get, set
    member val Data: 'T = Unchecked.defaultof<'T> with get, set
    override this.ToString() = Json.serialize this

type Command<'T when 'T :> CqrsCommandDto>() =
    inherit Message<'T>()

type Event<'T when 'T :> CqrsEventDto>() =
    inherit Message<'T>()

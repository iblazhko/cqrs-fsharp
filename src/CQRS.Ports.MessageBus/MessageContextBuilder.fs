module CQRS.Ports.Messaging.MessageContextBuilder

let getNewMessageContext clock =
    Context(
        MessageId = MessagingId.newId (),
        CorrelationId = MessagingId.newId (),
        CausationId = MessagingId.Empty,
        Timestamp = clock ()
    )

let getFromMessageContext (messageContext: Context) clock =
    Context(
        MessageId = MessagingId.newId (),
        CorrelationId = messageContext.CorrelationId,
        CausationId = messageContext.MessageId,
        Timestamp = clock ()
    )

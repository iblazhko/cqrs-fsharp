module CQRS.Ports.Messaging.MessageContextBuilder

let getNewMessageContext timestamp =
    Context(
        MessageId = MessagingId.newId (),
        CorrelationId = MessagingId.newId (),
        CausationId = MessagingId.Empty,
        Timestamp = timestamp
    )

let getFromMessageContext (messageContext: Context) timestamp =
    Context(
        MessageId = MessagingId.newId (),
        CorrelationId = messageContext.CorrelationId,
        CausationId = messageContext.MessageId,
        Timestamp = timestamp
    )

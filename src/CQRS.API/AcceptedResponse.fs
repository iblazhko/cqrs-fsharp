namespace CQRS.API

type AcceptedResponse =
    { EntityId: string
      MessageId: string
      CorrelationId: string
      CausationId: string
      Timestamp: string }

module AcceptedResponse =
    let fromEntityId (context: CQRS.DTO.Context) entityId =
        { EntityId = entityId
          MessageId = context.MessageId.ToString()
          CorrelationId = context.CorrelationId.ToString()
          CausationId = context.CausationId.ToString()
          Timestamp = context.Timestamp.ToString("O") }

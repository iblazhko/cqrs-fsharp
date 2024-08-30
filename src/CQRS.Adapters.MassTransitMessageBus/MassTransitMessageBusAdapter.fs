namespace CQRS.Adapters.Messaging

open System
open System.Threading
open System.Threading.Tasks
open CQRS.Ports.Messaging
open MassTransit

type private ContextDelegate = delegate of MessageContext -> unit

[<Sealed>]
type MassTransitMessageBusAdapter(sendEndpoint: ISendEndpointProvider, publishEndpoint: IPublishEndpoint) =
    let setPublishContext (publishContext: PublishContext) (context: CQRS.Ports.Messaging.Context option) =
        match context with
        | Some x ->
            publishContext.MessageId <- x.MessageId
            publishContext.CorrelationId <- x.CorrelationId
            publishContext.RequestId <- x.CausationId
            ()
        | None -> ()

    let publishEvent
        (message: obj)
        (context: CQRS.Ports.Messaging.Context option)
        (cancellationToken: CancellationToken option)
        =
        publishEndpoint.Publish(
            message,
            message.GetType(),
            callback = Action<PublishContext>(fun ctx -> context |> setPublishContext ctx),
            ?cancellationToken = cancellationToken
        )

    let setSendContext (sendContext: SendContext) (context: CQRS.Ports.Messaging.Context option) =
        match context with
        | Some x ->
            sendContext.MessageId <- x.MessageId
            sendContext.CorrelationId <- x.CorrelationId
            sendContext.RequestId <- x.CausationId
            ()
        | None -> ()

    let sendCommand
        (message: obj)
        (context: CQRS.Ports.Messaging.Context option)
        (cancellationToken: CancellationToken option)
        =
        task {
            let messageType = message.GetType()

            let found, destinationAddress =
                EndpointConvention.TryGetDestinationAddress(messageType)

            if not found then
                raise (
                    ArgumentException(
                        $"Sending convention for the message type {messageType.Name} was not found",
                        nameof message
                    )
                )

            let! endpoint = sendEndpoint.GetSendEndpoint(destinationAddress)

            return!
                endpoint.Send(
                    message,
                    messageType,
                    callback = Action<SendContext>(fun ctx -> context |> setSendContext ctx),
                    ?cancellationToken = cancellationToken
                )
        }
        :> Task

    interface IMessageBus with

        member this.PublishEvent<'TEvent>
            (
                message: CQRS.Ports.Messaging.Event<'TEvent>,
                cancellationToken: CancellationToken option
            ) : Task =
            publishEvent message.Data (Some message.Context) cancellationToken

        member this.PublishEvent
            (
                message: obj,
                context: CQRS.Ports.Messaging.Context option,
                cancellationToken: CancellationToken option
            ) : Task =
            publishEvent message context cancellationToken

        member this.SendCommand<'TCommand>(message: Command<'TCommand>, cancellationToken: CancellationToken option) =
            sendCommand message.Data (Some message.Context) cancellationToken

        member this.SendCommand
            (
                message: obj,
                context: CQRS.Ports.Messaging.Context option,
                cancellationToken: CancellationToken option
            ) : Task =
            sendCommand message context cancellationToken

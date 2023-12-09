namespace CQRS.Adapters.Messaging

open System.Threading.Tasks
open CQRS.Ports.Messaging

[<Sealed>]
type MassTransitMessageBusAdapter
    (sendEndpoint: MassTransit.ISendEndpointProvider, publishEndpoint: MassTransit.IPublishEndpoint) =
    interface IMessageBus with

        member this.PublishEvent<'TEvent>(message: Event<'TEvent>) : Task =
            task { do! publishEndpoint.Publish(message.Data, message.Data.GetType()) }

        member this.PublishEvent(message: obj) : Task =
            task { do! publishEndpoint.Publish(message, message.GetType()) }

        member this.SendCommand<'TCommand>(message: Command<'TCommand>) =
            MassTransit.EndpointConventionExtensions.Send(sendEndpoint, message.Data, message.Data.GetType())

        member this.SendCommand(message: obj) : Task =
            MassTransit.EndpointConventionExtensions.Send(sendEndpoint, message, message.GetType())

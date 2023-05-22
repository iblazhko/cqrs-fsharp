namespace CQRS.Adapters

open System.Threading.Tasks
open CQRS.DTO
open CQRS.Ports.Messaging
open MassTransit

type MassTransitMessageBusAdapter(sendEndpoint: ISendEndpointProvider, publishEndpoint: IPublishEndpoint) =
    interface IMessageBus with

        member this.PublishEvent<'TEvent when 'TEvent :> CqrsDto>(message: Message<'TEvent>) : Task =
            task { do! publishEndpoint.Publish(message.Data, message.Data.GetType()) }

        member this.PublishEvent(message: obj) : Task =
            task { do! publishEndpoint.Publish(message, message.GetType()) }

        member self.SendCommand<'TCommand when 'TCommand :> CqrsCommandDto>(message: Message<'TCommand>) =
            EndpointConventionExtensions.Send(sendEndpoint, message.Data, message.Data.GetType())

        member this.SendCommand(message: obj) : Task =
            EndpointConventionExtensions.Send(sendEndpoint, message, message.GetType())

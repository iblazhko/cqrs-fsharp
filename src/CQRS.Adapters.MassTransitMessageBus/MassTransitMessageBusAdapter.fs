namespace CQRS.Adapters

open CQRS.DTO
open CQRS.Ports.Messaging
open MassTransit

type MassTransitMessageBusAdapter(sendEndpoint: ISendEndpointProvider, publishEndpoint: IPublishEndpoint) =
    interface IMessageBus with

        member this.PublishEvent<'TEvent when 'TEvent :> CqrsDto>
            (message: Message<'TEvent>)
            : System.Threading.Tasks.Task =
            task {
                do! publishEndpoint.Publish(message.Data, message.Data.GetType())
                return ()
            }

        member this.PublishEvent(message: obj) : System.Threading.Tasks.Task =
            task {
                do! publishEndpoint.Publish(message, message.GetType())
                return ()
            }

        member self.SendCommand<'TCommand when 'TCommand :> CqrsCommandDto>(message: Message<'TCommand>) =
            EndpointConventionExtensions.Send(sendEndpoint, message.Data, message.Data.GetType())

        member this.SendCommand(message: obj) : System.Threading.Tasks.Task =
            EndpointConventionExtensions.Send(sendEndpoint, message, message.GetType())

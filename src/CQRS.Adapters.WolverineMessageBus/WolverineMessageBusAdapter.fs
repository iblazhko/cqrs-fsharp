namespace CQRS.Adapters.Messaging

open System.Threading
open System.Threading.Tasks
open CQRS.Ports.Messaging

[<Sealed>]
type WolverineMessageBusAdapter(bus: Wolverine.IMessageBus) =

    let toDeliveryOptions (context: Context option) =
        let opts = Wolverine.DeliveryOptions()

        match context with
        | Some ctx ->
            opts.Headers["id"] <- ctx.MessageId.ToString()
            opts.Headers["correlation-id"] <- ctx.CorrelationId.ToString()
            opts.Headers["causation-id"] <- ctx.CausationId.ToString()
        | None -> ()

        opts

    let publishEvent (message: obj) (context: Context option) =
        bus.PublishAsync(message, toDeliveryOptions context).AsTask()

    let sendCommand (message: obj) (context: Context option) =
        bus.SendAsync(message, toDeliveryOptions context).AsTask()

    interface IMessageBus with

        member _.PublishEvent<'TEvent>
            (
                message: Event<'TEvent>,
                _: CancellationToken option
            ) : Task =
            publishEvent message.Data (Some message.Context)

        member _.PublishEvent
            (
                message: obj,
                context: Context option,
                _: CancellationToken option
            ) : Task =
            publishEvent message context

        member _.SendCommand<'TCommand>
            (
                message: Command<'TCommand>,
                _: CancellationToken option
            ) : Task =
            sendCommand message.Data (Some message.Context)

        member _.SendCommand
            (
                message: obj,
                context: Context option,
                _: CancellationToken option
            ) : Task =
            sendCommand message context

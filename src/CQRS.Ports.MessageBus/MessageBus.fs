namespace CQRS.Ports.Messaging

open System.Threading
open System.Threading.Tasks

type IPublishEvent =
    abstract member PublishEvent: Event<'TEvent> * CancellationToken option -> Task
    abstract member PublishEvent: obj * Context option * CancellationToken option -> Task

type ISendCommand =
    abstract member SendCommand: Command<'TCommand> * CancellationToken option -> Task
    abstract member SendCommand: obj * Context option * CancellationToken option -> Task

type IMessageBus =
    inherit IPublishEvent
    inherit ISendCommand

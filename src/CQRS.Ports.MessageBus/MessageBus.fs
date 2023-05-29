namespace CQRS.Ports.Messaging

open System.Threading.Tasks

type IPublishEvent =
    abstract member PublishEvent: Event<'TEvent> -> Task
    abstract member PublishEvent: obj -> Task

type ISendCommand =
    abstract member SendCommand: Command<'TCommand> -> Task
    abstract member SendCommand: obj -> Task

type IMessageBus =
    inherit IPublishEvent
    inherit ISendCommand

namespace CQRS.Ports.Messaging

open System.Threading.Tasks
open CQRS.DTO

type IPublishEvent =
    abstract member PublishEvent: Message<'TEvent> -> Task when 'TCommand :> CqrsEventDto
    abstract member PublishEvent: obj -> Task

type ISendCommand =
    abstract member SendCommand: Message<'TCommand> -> Task when 'TCommand :> CqrsCommandDto
    abstract member SendCommand: obj -> Task

type IMessageBus =
    inherit IPublishEvent
    inherit ISendCommand

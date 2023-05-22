namespace CQRS.Application

open CQRS.Ports.EventStore
open CQRS.Ports.Messaging

type EventStoreEventsPublisher(bus: IMessageBus) =
    interface IEventPublisher with
        member this.Publish(events) =
            task {
                for e in events do
                    do! bus.PublishEvent(e.Event)

                return ()
            }

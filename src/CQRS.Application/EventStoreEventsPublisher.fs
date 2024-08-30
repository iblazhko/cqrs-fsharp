namespace CQRS.Application

open CQRS.Ports.EventStore
open CQRS.Ports.Messaging

type EventStoreEventsPublisher(bus: IMessageBus) =
    interface IEventPublisher with
        member this.Publish(events, cancellationToken) =
            task {
                for e in events do
                    do!
                        bus.PublishEvent(
                            e.Event,
                            e.Metadata
                            |> Option.map (fun x ->
                                let ctx = Context()
                                ctx.MessageId <- x.EventId
                                ctx.CorrelationId <- x.CorrelationId
                                ctx.CausationId <- x.CausationId
                                ctx),
                            cancellationToken
                        )
            }

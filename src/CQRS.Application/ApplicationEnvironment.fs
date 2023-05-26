namespace CQRS.Application

open CQRS.Ports.EventStore
open CQRS.Ports.Time

type ApplicationEnvironment =
    { Location: Location
      Clock: IClock
      EventStore: IEventStore
      MoonPhase: IMoonPhaseService }

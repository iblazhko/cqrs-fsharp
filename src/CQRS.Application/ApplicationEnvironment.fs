namespace CQRS.Application

open System
open CQRS.Application.CommandProcessingStatusRecording
open CQRS.Ports.EventStore

type ApplicationEnvironment =
    { Location: Location
      Clock: TimeProvider
      EventStore: IEventStore
      MoonPhase: IMoonPhaseService
      CommandProcessingStatusRecorder: ICommandProcessingStatusRecordingService }

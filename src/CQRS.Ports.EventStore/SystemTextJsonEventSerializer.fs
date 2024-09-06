namespace CQRS.Ports.EventStore

open System.Text.Json

type SystemTextJsonEventSerializer(?options: JsonSerializerOptions) =
    interface IEventSerializer with
        member this.Serialize(eventWithType) =
            let evt, evtType = eventWithType
            JsonSerializer.Serialize(evt, evtType, ?options = options)

        member this.Deserialize(eventWithType) =
            let evt, evtType = eventWithType
            JsonSerializer.Deserialize(evt, evtType, ?options = options)

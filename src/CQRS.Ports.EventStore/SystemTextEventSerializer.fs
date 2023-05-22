namespace CQRS.Ports.EventStore

open System.Text.Json

type SystemTextEventSerializer(?options: JsonSerializerOptions) =
    let optionsOrNull =
        match options with
        | Some o -> o
        | None -> null

    interface IEventSerializer with
        member this.Serialize(eventWithType) =
            let evt, evtType = eventWithType
            JsonSerializer.Serialize(evt, evtType, optionsOrNull)

        member this.Deserialize(eventWithType) =
            let evt, evtType = eventWithType
            JsonSerializer.Deserialize(evt, evtType, optionsOrNull)

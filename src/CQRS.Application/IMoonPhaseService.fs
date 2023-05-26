namespace CQRS.Application

open System
open System.Threading.Tasks
open CQRS.Domain.Moon

type Location = string

[<Interface>]
type IMoonPhaseService =
    abstract member GetMoonPhase: Location * DateTimeOffset -> Task<MoonPhase>

type MoonPhaseServiceStub() =
    interface IMoonPhaseService with
        member this.GetMoonPhase(_, timestamp) =
            task {
                let phase =
                    match timestamp.Hour with
                    | h when h >= 6 && h < 11 -> MoonPhase.FirstQuarter
                    | h when h >= 11 && h < 15 -> MoonPhase.FullMoon
                    | h when h >= 15 && h < 21 -> MoonPhase.LastQuarter
                    | _ -> MoonPhase.NewMoon

                return phase
            }

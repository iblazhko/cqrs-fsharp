namespace CQRS.Ports.Time

open System

[<Interface>]
type IClock =
    abstract member Now: unit -> DateTimeOffset

// Given that system clock implementation does not have any external dependencies,
// we define it in-place to save us from creating another project with 1 file.
// Application services should still depend on IClock interface, not on this implementation.
type SystemClock() =
    interface IClock with
        member this.Now() = DateTimeOffset.Now

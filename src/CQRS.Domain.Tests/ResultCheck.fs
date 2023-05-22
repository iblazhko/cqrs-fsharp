module CQRS.Domain.Tests.ResultCheck

open FPrimitive

let failIfError (x: Result<'T, ErrorsByTag>) =
    match x with
    | Ok _ -> ()
    | Error e ->
        failwith (
            e
            |> Map.toSeq
            |> Seq.map snd
            |> Seq.collect Seq.ofList
            |> Seq.toList
            |> sprintf "%A"
        )

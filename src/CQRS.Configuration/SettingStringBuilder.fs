module CQRS.Configuration.SettingStringBuilder

open System
open System.Text

[<Literal>]
let private indent = "  "

[<Literal>]
let private defaultValue = "<NOT SET>"

[<Literal>]
let private redactedValue = "<REDACTED>"

let private stringOrDefault<'a> (value: 'a) =
    match box value with
    | null -> defaultValue
    | _ ->
        match value.ToString() with
        | x when String.IsNullOrWhiteSpace(x) -> defaultValue
        | x -> x

let private redactedOrDefault<'a> (value: 'a) =
    match box value with
    | null -> defaultValue
    | _ ->
        match value.ToString() with
        | x when String.IsNullOrWhiteSpace(x) -> defaultValue
        | _ -> redactedValue

let appendSettingsTitle (name: string) (builder: StringBuilder) =
    if String.IsNullOrWhiteSpace(name) then
        failwith "Name must be specified"

    builder.AppendLine($"{name}".ToUpperInvariant()) |> ignore
    builder.AppendLine("=====") |> ignore
    builder


let appendSettingSectionTitle (name: string) (builder: StringBuilder) =
    if String.IsNullOrWhiteSpace(name) then
        failwith "Name must be specified"

    builder.AppendLine($"{indent}{name}") |> ignore
    builder.AppendLine($"{indent}-----") |> ignore
    builder

let appendSettingValue<'a> (value: 'a) (name: string) (builder: StringBuilder) =
    if String.IsNullOrWhiteSpace(name) then
        failwith "Name must be specified"

    let formattedValue = value |> stringOrDefault
    builder.AppendLine($"{indent}{name}: {formattedValue}") |> ignore

    builder

let appendSettingRedactedValue<'a> (value: 'a) (name: string) (builder: StringBuilder) =
    if String.IsNullOrWhiteSpace(name) then
        failwith "Name must be specified"

    let formattedValue = value |> redactedOrDefault
    builder.AppendLine($"{indent}{name}: {formattedValue}") |> ignore

    builder

let appendSettingSection<'a> (value: 'a) (name: string) (builder: StringBuilder) =
    if String.IsNullOrWhiteSpace(name) then
        failwith "Name must be specified"

    builder.AppendLine(String.Empty) |> ignore

    let formattedValue = value |> stringOrDefault
    appendSettingSectionTitle name builder |> ignore

    formattedValue.Split('\n')
    |> Array.map (fun l ->
        if not (String.IsNullOrWhiteSpace(l)) then
            builder.AppendLine($"{indent}{indent}{l}") |> ignore)
    |> ignore

    builder

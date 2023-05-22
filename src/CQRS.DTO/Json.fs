module CQRS.DTO.Json

open System.Text.Json

let private serializeOptions = JsonSerializerOptions(WriteIndented = true)

let serialize obj =
    JsonSerializer.Serialize(obj, serializeOptions)

let deserialize<'a> (str: string) =
    try
        JsonSerializer.Deserialize<'a> str |> Result.Ok
    with ex ->
        Result.Error ex

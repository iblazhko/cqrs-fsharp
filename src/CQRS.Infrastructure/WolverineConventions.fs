module CQRS.Infrastructure.WolverineConventions

open System
open System.Reflection
open CQRS.Configuration.Infrastructure
open CQRS.DTO
open Wolverine
open Wolverine.RabbitMQ

let private typeOfCommandDto = typedefof<CqrsCommandDto>
let private envelopeType = typeof<Wolverine.Envelope>

let private removeStringSuffix (suffixes: string array) (s: string) =
    match suffixes |> Seq.tryFind (fun x -> s.EndsWith(x, StringComparison.Ordinal)) with
    | Some x -> s.Substring(0, s.Length - x.Length)
    | None -> s

let private toQueueName (queuePrefix: string) (typeName: string) =
    // Replicate snake_case conversion matching PrefixedSnakeCaseEndpointNameFormatter logic:
    // "CreateInventoryCommand" -> "cqrs:create_inventory"
    let snakeCase =
        typeName
        |> Seq.fold
            (fun (acc: System.Text.StringBuilder) c ->
                if Char.IsUpper(c) && acc.Length > 0 then acc.Append('_').Append(Char.ToLower(c))
                else acc.Append(Char.ToLower(c)))
            (System.Text.StringBuilder())
        |> fun b -> b.ToString()

    let trimmed = snakeCase |> removeStringSuffix [| "_command"; "_event" |]
    $"{queuePrefix}:{trimmed}"

// Scan handler assemblies for message types by inspecting Handle/HandleAsync method signatures.
// Consumer assemblies contain handler classes, not DTO types — the DTO types live in CQRS.DTO.
let private messageTypesFromHandlerAssembly (asm: Assembly) =
    asm.GetTypes()
    |> Array.toSeq
    |> Seq.filter (fun t -> not t.IsAbstract && t.IsPublic)
    |> Seq.collect (fun t ->
        t.GetMethods(BindingFlags.Public ||| BindingFlags.Instance)
        |> Array.toSeq
        |> Seq.filter (fun m -> m.Name = "Handle" || m.Name = "HandleAsync")
        |> Seq.choose (fun m ->
            m.GetParameters()
            |> Array.tryFind (fun p ->
                not (envelopeType.IsAssignableFrom(p.ParameterType))
                && not p.ParameterType.IsValueType
                && p.ParameterType <> typeof<System.Threading.CancellationToken>))
        |> Seq.map (fun p -> p.ParameterType))
    |> Seq.distinct

let private commandTypesFromAssembly (asm: Assembly) =
    asm.GetTypes()
    |> Array.toSeq
    |> Seq.filter (fun t -> not t.IsAbstract && typeOfCommandDto.IsAssignableFrom(t))

let registerListeners (opts: WolverineOptions) (settings: WolverineSettings) (queuePrefix: string) (assemblies: System.Reflection.Assembly list) =
    assemblies
    |> List.iter (fun asm ->
        messageTypesFromHandlerAssembly asm
        |> Seq.iter (fun messageType ->
            let queueName = toQueueName queuePrefix messageType.Name
            RabbitMqTransportExtensions.ListenToRabbitQueue(opts, queueName) |> ignore))

let registerSendConventions (opts: WolverineOptions) (settings: WolverineSettings) (queuePrefix: string) (assemblies: System.Reflection.Assembly list) =
    assemblies
    |> List.iter (fun asm ->
        commandTypesFromAssembly asm
        |> Seq.iter (fun commandType ->
            let queueName = toQueueName queuePrefix commandType.Name
            opts.PublishMessage(commandType).ToRabbitQueue(queueName) |> ignore))

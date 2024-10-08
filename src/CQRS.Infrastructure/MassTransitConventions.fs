module CQRS.Infrastructure.MassTransitConventions

open System
open System.Reflection
open CQRS.Configuration.Infrastructure
open CQRS.DTO
open CQRS.Ports.Messaging
open MassTransit

let private typeOfMessageEnvelope = typedefof<Command<_>>
let private typeOfCommandDto = typedefof<CqrsCommandDto>
let private typeofEndpointConvention = typedefof<EndpointConvention>
let private typeOfUri = typedefof<Uri>

let private removeStringSuffix (suffixes: string array) (s: string) =
    match suffixes |> Seq.tryFind (fun x -> s.EndsWith(x, StringComparison.Ordinal)) with
    | Some x -> s.Substring(0, s.Length - x.Length)
    | None -> s

type PrefixedSnakeCaseEndpointNameFormatter(prefix: string) =
    inherit SnakeCaseEndpointNameFormatter()

    override self.SanitizeName(name: string) =
        base.SanitizeName $"{prefix}:{name}"
        |> removeStringSuffix [| "_command"; "_event" |]


let private methodEndpointConventionMap =
    typeofEndpointConvention.GetMethod("Map", [| typeOfUri |])

let registerSendConventionForType (settings: MassTransitSettings) (queuePrefix: string) (commandType: Type) =
    let formatter = PrefixedSnakeCaseEndpointNameFormatter(queuePrefix)

    let queueAddress =
        Uri($"{settings.RabbitMq.getRabbitMqUrl ()}/{formatter.SanitizeName(commandType.Name)}")

    let commandMapMethod = methodEndpointConventionMap.MakeGenericMethod(commandType)

    commandMapMethod.Invoke(null, [| queueAddress |]) |> ignore


let registerSendConventionForTypes (settings: MassTransitSettings) (queuePrefix: string) (types: Type seq) =
    types |> Seq.iter (registerSendConventionForType settings queuePrefix)

let registerSendConventionForAssembly (settings: MassTransitSettings) (queuePrefix: string) (asm: Assembly) =
    asm.GetTypes()
    |> Array.toSeq
    |> Seq.filter (fun x -> (not x.IsAbstract) && typeOfCommandDto.IsAssignableFrom(x))
    |> registerSendConventionForTypes settings queuePrefix

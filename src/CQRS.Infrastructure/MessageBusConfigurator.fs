module CQRS.Infrastructure.MessageBusConfigurator

open CQRS.Adapters.Messaging
open CQRS.Infrastructure.WolverineConventions
open Microsoft.Extensions.DependencyInjection
open Wolverine
open CQRS.Configuration.Infrastructure
open CQRS.Ports.Messaging

let configureServices
    (endpointsRegistration: HostEndpointsRegistration)
    (settings: WolverineSettings)
    (services: IServiceCollection)
    =
    services.AddWolverine(fun (opts: WolverineOptions) ->
        Wolverine.RabbitMQ.RabbitMqTransportExtensions.UseRabbitMq(
            opts,
            fun factory ->
                factory.HostName <- settings.RabbitMq.Endpoint.Host
                factory.Port <- settings.RabbitMq.Endpoint.Port
                factory.UserName <- settings.RabbitMq.Username
                factory.Password <- settings.RabbitMq.Password
                factory.VirtualHost <- settings.RabbitMq.VirtualHost)
            .AutoProvision()
        |> ignore

        match endpointsRegistration.Consumers with
        | Some e ->
            e.assemblies |> List.iter (fun asm -> opts.Discovery.IncludeAssembly(asm) |> ignore)
            registerListeners opts settings e.queuePrefix e.assemblies
        | None -> ()

        match endpointsRegistration.SendConventions with
        | Some c -> registerSendConventions opts settings c.queuePrefix c.assemblies
        | None -> ())
    |> ignore

    services.AddSingleton<IMessageBus>(fun serviceProvider ->
        WolverineMessageBusAdapter(serviceProvider.GetRequiredService<Wolverine.IMessageBus>()) :> IMessageBus)
    |> ignore

    services

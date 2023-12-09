module CQRS.Infrastructure.MessageBusConfigurator

open CQRS.Adapters.Messaging
open CQRS.Infrastructure.MassTransitConventions
open Microsoft.Extensions.DependencyInjection
open MassTransit
open CQRS.Configuration.Infrastructure
open CQRS.Ports.Messaging

let configureServices
    (endpointsRegistration: HostEndpointsRegistration)
    (settings: MassTransitSettings)
    (services: IServiceCollection)
    =
    services.AddMassTransit(fun (configure: IBusRegistrationConfigurator) ->
        match endpointsRegistration.Consumers with
        | Some e -> e.assemblies |> List.iter configure.AddConsumers
        | None -> ()

        configure.UsingRabbitMq(fun registrationContext ->
            fun configurator ->
                configurator.Host(
                    settings.RabbitMq.Endpoint.Host,
                    settings.RabbitMq.VirtualHost,
                    fun host ->
                        host.Username settings.RabbitMq.Username
                        host.Password settings.RabbitMq.Password
                )

                match endpointsRegistration.Consumers with
                | Some e ->
                    let formatter = PrefixedSnakeCaseEndpointNameFormatter(e.queuePrefix)
                    configurator.ConfigureEndpoints(registrationContext, formatter)
                | None -> ()))
    |> ignore

    match endpointsRegistration.SendConventions with
    | Some c ->
        c.assemblies
        |> List.iter (registerSendConventionForAssembly settings c.queuePrefix)
    | None -> ()

    services.AddSingleton<IMessageBus>(fun serviceProvider ->
        let bus = serviceProvider.GetRequiredService<IBus>()
        MassTransitMessageBusAdapter(bus, bus) :> IMessageBus)
    |> ignore

    services

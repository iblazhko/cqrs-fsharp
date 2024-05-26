module CQRS.API.Host.Program

open CQRS.Configuration
open CQRS.DTO
open CQRS.Infrastructure
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting

open CQRS.API.ApiRoutes
open CQRS.Infrastructure.Startup

[<EntryPoint>]
let main (args: string[]) =
    let settings = CqrsSettingsLoader.getHostSettings "CqrsApi" args
    printStartMessage "CQRS API Host" settings
    waitForInfrastructure settings

    let builder = WebApplication.CreateBuilder(args)

    let endpointsRegistration =
        { SendConventions =
            Some
                { assemblies = [ typedefof<CqrsDto>.Assembly ]
                  queuePrefix = "cqrs" }
          Consumers = None }

    builder.WebHost.ConfigureServices(configureServices endpointsRegistration settings)
    |> ignore

    builder.WebHost.ConfigureLogging(configureLogging settings.Logging) |> ignore
    builder.WebHost.UseUrls(settings.ServiceUrl) |> ignore

    let app = builder.Build()
    app |> configureApp settings
    app |> configureApiRoutes

    app.Run()
    0

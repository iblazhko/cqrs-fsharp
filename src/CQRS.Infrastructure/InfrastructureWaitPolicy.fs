module CQRS.Infrastructure.InfrastructureWaitPolicy

open System
open System.Net.Sockets
open Polly
open Polly.Contrib.WaitAndRetry
open CQRS.Configuration.Infrastructure

type InfrastructureService =
    { Name: string
      Endpoint: EndpointSettings }

type private ServiceAvailabilityResult =
    { ServiceName: string
      Host: string
      Port: int
      IsAvailable: bool }

let private isPortOpen (host: string) (port: int) (timeout: TimeSpan) : bool =
    try
        use client = new TcpClient()
        let result = client.BeginConnect(host, port, null, null)
        let success = result.AsyncWaitHandle.WaitOne timeout
        client.EndConnect result
        success
    with _ ->
        false

let private portCheckTimeout = TimeSpan.FromSeconds(3.0)

let private getServiceAvailability (service: InfrastructureService) =
    { ServiceName = service.Name
      Host = service.Endpoint.Host
      Port = service.Endpoint.Port
      IsAvailable = (isPortOpen service.Endpoint.Host service.Endpoint.Port portCheckTimeout) }

let private isInfrastructureAvailable (services: InfrastructureService list) =
    let infrastructureServicesAvailability = services |> List.map getServiceAvailability

    let unavailableMessages =
        infrastructureServicesAvailability
        |> Seq.where (fun x -> (not x.IsAvailable))
        |> Seq.map (fun x -> $"Service {x.ServiceName} is not available at {x.Host}:{x.Port}")
        |> Seq.toArray

    if (unavailableMessages.Length > 0) then
        let timestamp = DateTime.UtcNow.ToString("o")
        let errorMessage = unavailableMessages |> String.concat "; "
        printfn $"%s{timestamp}: %s{errorMessage}"

    (unavailableMessages.Length = 0)

let waitForInfrastructure (settings: InfrastructureStartupSettings) (services: InfrastructureService list) =
    if settings.WaitOnStartup then
        let policy =
            Policy
                .HandleResult(false)
                .WaitAndRetry(Backoff.DecorrelatedJitterBackoffV2(settings.RetryDelay, settings.RetryCount))

        policy.Execute(fun () -> (isInfrastructureAvailable services))
    else
        true

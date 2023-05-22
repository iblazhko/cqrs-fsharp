module CQRS.CLI.ApiClient

open CQRS.CLI.FlurlFsharp
open CQRS.DTO.V1
open Serilog

let createInventoryItem name apiServiceUrl =
    let dto = CreateInventoryItemCommand()
    dto.Name <- name
    Log.Logger.Information("[PUT>] {@Url}/api/v1/inventories {@Command}", apiServiceUrl, dto)

    let response =
        apiServiceUrl |> appendPathSegment "/api/v1/inventories" |> putJson dto

    let responseStatusCode = response.ResponseMessage.StatusCode
    let responseBody = response.GetStringAsync().Result

    Log.Logger.Information(
        "[PUT<] /v1/inventories {@ResponseStatusCode} {@ResponseBody}",
        responseStatusCode,
        responseBody
    )

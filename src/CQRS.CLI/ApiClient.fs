module CQRS.CLI.ApiClient

open CQRS.CLI.FlurlFsharp
open CQRS.DTO.V1
open Serilog

let createInventory name apiServiceUrl =
    let dto = CreateInventoryCommand()
    dto.Name <- name
    Log.Logger.Information("[PUT>] {Url}/api/v1/inventories {@Command}", apiServiceUrl, dto)

    let response =
        apiServiceUrl |> appendPathSegment "/api/v1/inventories" |> putJson dto

    let responseStatusCode = response.ResponseMessage.StatusCode
    let responseBody = response.GetStringAsync().Result

    Log.Logger.Information(
        "[PUT<] {Url}/v1/inventories {ResponseStatusCode} {ResponseBody}",
        apiServiceUrl,
        responseStatusCode,
        responseBody
    )

let getInventory id apiServiceUrl =
    Log.Logger.Information("[GET>] {Url}/api/v1/inventories/{Id}", apiServiceUrl, id)

    let response =
        apiServiceUrl |> appendPathSegment $"/api/v1/inventories/{id}" |> getJson

    let responseStatusCode = response.ResponseMessage.StatusCode
    let responseBody = response.GetStringAsync().Result

    Log.Logger.Information(
        "[GET<] {Url}/v1/inventories/{Id} {ResponseStatusCode} {ResponseBody}",
        apiServiceUrl,
        id,
        responseStatusCode,
        responseBody
    )

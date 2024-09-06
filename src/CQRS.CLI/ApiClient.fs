module CQRS.CLI.ApiClient

open CQRS.CLI.FlurlFsharp
open CQRS.DTO.V1
open Flurl
open Serilog

[<Literal>]
let private inventoriesEndpoint = "/v1/inventories"

let private getWithResponseBody (url: Url) =
    Log.Logger.Information("[GET>] {Url}", url)

    let response = url |> getJson
    let responseStatusCode = response.ResponseMessage.StatusCode
    let responseBody = response.GetStringAsync().Result

    Log.Logger.Information("[GET<] {Url} {ResponseStatusCode} {@ResponseBody}", url, responseStatusCode, responseBody)

let private postWithBody<'a> (body: 'a) (url: Url) =
    Log.Logger.Information("[POST>] {Url} {@RequestBody}", url, body)

    let response = url |> postJson body
    let responseStatusCode = response.ResponseMessage.StatusCode
    let responseBody = response.GetStringAsync().Result

    Log.Logger.Information("[POST<] {Url} {ResponseStatusCode} {@ResponseBody}", url, responseStatusCode, responseBody)

let private postWithEmptyBody (url: Url) =
    Log.Logger.Information("[POST>] {Url}", url)

    let response = url |> post
    let responseStatusCode = response.ResponseMessage.StatusCode
    let responseBody = response.GetStringAsync().Result

    Log.Logger.Information("[POST<] {Url} {ResponseStatusCode} {@ResponseBody}", url, responseStatusCode, responseBody)

let createInventory (name: string) (apiServiceUrl: string) =
    let dto = CreateInventoryCommand()
    dto.Name <- name

    apiServiceUrl
    |> appendPathSegmentToString inventoriesEndpoint
    |> postWithBody dto

let renameInventory (id: string) (name: string) apiServiceUrl =
    apiServiceUrl
    |> appendPathSegmentToString inventoriesEndpoint
    |> appendPathSegmentToUrl id
    |> appendPathSegmentToUrl "rename"
    |> appendEncodedPathSegmentToUrl name
    |> postWithEmptyBody

let addItemsToInventory (id: string) (count: int) apiServiceUrl =
    apiServiceUrl
    |> appendPathSegmentToString inventoriesEndpoint
    |> appendPathSegmentToUrl id
    |> appendPathSegmentToUrl "add"
    |> appendPathSegmentToUrl $"{count}"
    |> postWithEmptyBody

let removeItemsFromInventory (id: string) (count: int) apiServiceUrl =
    apiServiceUrl
    |> appendPathSegmentToString inventoriesEndpoint
    |> appendPathSegmentToUrl id
    |> appendPathSegmentToUrl "remove"
    |> appendPathSegmentToUrl $"{count}"
    |> postWithEmptyBody

let deactivateInventory (id: string) apiServiceUrl =
    apiServiceUrl
    |> appendPathSegmentToString inventoriesEndpoint
    |> appendPathSegmentToUrl id
    |> appendPathSegmentToUrl "deactivate"
    |> postWithEmptyBody

let getInventory id apiServiceUrl =
    apiServiceUrl
    |> appendPathSegmentToString inventoriesEndpoint
    |> appendPathSegmentToUrl id
    |> getWithResponseBody

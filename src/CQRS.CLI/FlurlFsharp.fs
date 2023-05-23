module CQRS.CLI.FlurlFsharp

open System.Net.Http
open Flurl
open Flurl.Http

let appendPathSegmentToString (segment: string) (url: string) =
    GeneratedExtensions.AppendPathSegment(url, segment)

let appendPathSegmentToUrl (segment: string) (url: Url) =
    GeneratedExtensions.AppendPathSegment(url, segment)

let appendEncodedPathSegmentToString (segment: string) (url: string) =
    GeneratedExtensions.AppendPathSegment(url, segment, true)

let appendEncodedPathSegmentToUrl (segment: string) (url: Url) =
    GeneratedExtensions.AppendPathSegment(url, segment, true)

let post (endpoint: Url) =
    FlurlRequest(endpoint).AllowAnyHttpStatus().PostAsync().Result

let postJson x (endpoint: Url) =
    FlurlRequest(endpoint).AllowAnyHttpStatus().PostJsonAsync(x).Result

let putJson x (endpoint: Url) =
    FlurlRequest(endpoint).AllowAnyHttpStatus().PutJsonAsync(x).Result

let getJson (endpoint: Url) =
    FlurlRequest(endpoint).AllowAnyHttpStatus().SendAsync(HttpMethod.Get).Result

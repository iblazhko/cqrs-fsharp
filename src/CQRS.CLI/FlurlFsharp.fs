module CQRS.CLI.FlurlFsharp

open Flurl
open Flurl.Http

let appendPathSegment (segment: string) (url: string) =
    GeneratedExtensions.AppendPathSegment(url, segment)

let appendEncodedPathSegment (segment: string) (url: string) =
    GeneratedExtensions.AppendPathSegment(url, segment, true)

let postJson x (endpoint: Url) =
    FlurlRequest(endpoint).AllowAnyHttpStatus().PostJsonAsync(x).Result

let putJson x (endpoint: Url) =
    FlurlRequest(endpoint).AllowAnyHttpStatus().PutJsonAsync(x).Result

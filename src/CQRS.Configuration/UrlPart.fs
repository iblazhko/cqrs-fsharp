module CQRS.Configuration.UrlPart

open System

let toUrlPart s =
    if String.IsNullOrWhiteSpace s then
        String.Empty
    else
        Uri.EscapeDataString s

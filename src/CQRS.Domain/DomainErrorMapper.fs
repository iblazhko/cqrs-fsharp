module CQRS.Domain.DomainErrorMapper

open FPrimitive

let mapDomainError name domainMessage =
    ErrorsByTag(Seq.singleton (name, [ $"%A{domainMessage}" ]))

let mapStringError (errors: ErrorsByTag) =
    System.Text.Json.JsonSerializer.Serialize(errors)

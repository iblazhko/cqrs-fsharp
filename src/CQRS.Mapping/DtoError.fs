namespace CQRS.Mapping

open System
open FPrimitive

type DtoError =
    | ValidationError of ErrorsByTag
    | DeserializationException of Exception

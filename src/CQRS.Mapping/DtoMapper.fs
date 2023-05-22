namespace CQRS.Mapping

open System
open FPrimitive
open FsToolkit.ErrorHandling

type DtoError =
    | ValidationError of ErrorsByTag
    | DeserializationException of Exception

module DtoMapper =
    let ensureNotNull<'a when 'a: null> (dto: 'a) =
        if isNull dto then
            Error(ErrorsByTag(Seq.singleton (typedefof<'a>.Name, [ "DTO should not be null" ])))
        else
            Ok dto

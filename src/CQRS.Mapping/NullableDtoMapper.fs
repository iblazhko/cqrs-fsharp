namespace CQRS.Mapping

open FPrimitive
open FsToolkit.ErrorHandling

module NullableDtoMapper =
    let ofNullable<'Tdto when 'Tdto: null> (dto: 'Tdto) =
        if isNull dto then
            Error(ErrorsByTag(Seq.singleton (typedefof<'Tdto>.Name, [ "DTO should not be null" ])))
        else
            Ok dto

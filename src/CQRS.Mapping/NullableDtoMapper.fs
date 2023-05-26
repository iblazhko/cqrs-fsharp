namespace CQRS.Mapping

open FPrimitive

module NullableDtoMapper =
    let ofNullable<'Tdto when 'Tdto: null> (dto: 'Tdto) =
        if isNull dto then
            Error(ErrorsByTag(seq { (typedefof<'Tdto>.Name, [ "DTO should not be null" ]) }))
        else
            Ok dto

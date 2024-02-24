namespace CQRS.EntityIds

open System
open FPrimitive

// type alias shared with DTOs
type EntityIdRawValue = string

type EntityId = private EntityId of EntityIdRawValue

module EntityId =
    let private normalize (id: String) = id.ToUpperInvariant()

    let newId () = EntityId(Guid.NewGuid().ToString("N") |> normalize) // TODO: replace with shorter id

    let create propertyName (id: EntityIdRawValue) =
        Spec.def
        |> Spec.notEqual EntityIdRawValue.Empty (propertyName + " should be specified")
        |> Spec.createModel EntityId (id |> normalize)

    let value (EntityId id) = id

    let toString id = id |> value

    let fromString propertyName (idStr: string) =
        let success = not (String.IsNullOrWhiteSpace(idStr))

        if success then
            idStr |> create propertyName
        else
            Error(ErrorsByTag(seq { (propertyName, [ $"Could not parse Id value '{idStr}'" ]) }))

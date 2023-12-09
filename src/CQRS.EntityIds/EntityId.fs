namespace CQRS.EntityIds

open System
open FPrimitive

// type alias shared with DTOs
type EntityIdRawValue = Guid

type EntityId = private EntityId of EntityIdRawValue

module EntityId =
    let newId () = EntityId(EntityIdRawValue.NewGuid())

    let create propertyName (id: EntityIdRawValue) =
        Spec.def
        |> Spec.notEqual EntityIdRawValue.Empty (propertyName + " should be specified")
        |> Spec.createModel EntityId id

    let value (EntityId id) = id

    let toString id = (id |> value).ToString("N")

    let fromString propertyName (idStr: string) =
        let success, id = EntityIdRawValue.TryParse(idStr)

        if success then
            id |> create propertyName
        else
            Error(ErrorsByTag(seq { (propertyName, [ $"Could not parse Id value '{idStr}'" ]) }))

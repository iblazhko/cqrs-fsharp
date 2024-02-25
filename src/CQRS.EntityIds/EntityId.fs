namespace CQRS.EntityIds

open FPrimitive
open NanoidDotNet

// type alias shared with DTOs
type EntityIdRawValue = string

type EntityId = private EntityId of EntityIdRawValue

module EntityId =
    [<Literal>]
    let private idAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"

    [<Literal>]
    let private idLength = 12

    (*
    As per https://zelark.github.io/nano-id-cc/
    with the alphabet and length above at 1000 IDs / hour rate
    ~41 years or 363M IDs needed in order to have a 1% probability of at least one collision
    *)

    let private normalize (id: string) = id.ToUpperInvariant()

    let newId () = EntityId(Nanoid.Generate(idAlphabet, idLength))

    let create propertyName (id: EntityIdRawValue) =
        Spec.def
        |> Spec.notEmpty $"{propertyName} should be specified"
        |> Spec.length idLength $"{propertyName} should be {idLength} characters long"
        |> Spec.alphanum $"{propertyName} should only contain alphanumeric characters"
        |> Spec.createModel EntityId (id |> normalize)

    let value (EntityId id) = id


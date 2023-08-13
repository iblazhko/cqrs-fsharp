namespace CQRS.Domain.ValueTypes

(*
Building blocks for domain model.
These value types are domain-specific, but not entity-specific.

https://pragprog.com/book/swdddf/domain-modeling-made-functional
*)

open FPrimitive

type PositiveInteger = private PositiveInteger of int

module PositiveInteger =
    let create propertyName x =
        Spec.def
        |> Spec.greaterThanOrEqual 0 $"{propertyName} should be greater than zero"
        |> Spec.createModel PositiveInteger x

    let minValue () = PositiveInteger 1

    let increment x =
        let (PositiveInteger n) = x
        PositiveInteger(n + 1)

    let add x y =
        let (PositiveInteger n) = x
        let (PositiveInteger m) = y
        PositiveInteger(n + m)

    let subtract x y =
        let (PositiveInteger n) = x
        let (PositiveInteger m) = y

        if m < n then
            Ok(PositiveInteger(n - m))
        else
            Error $"Subtraction '{n}-{m}' produced a non-positive number"

    let greaterThan x y =
        let (PositiveInteger n) = x
        let (PositiveInteger m) = y
        n > m

    let equal x y =
        let (PositiveInteger n) = x
        let (PositiveInteger m) = y
        n = m

    let value (PositiveInteger x) = x

type ShortString = private ShortString of string

module ShortString =
    [<Literal>]
    let private maxLength = 10

    let create propertyName x =
        Spec.def
        |> Spec.notNull $"{propertyName} should be specified"
        |> Spec.notEmpty $"{propertyName} value should not be empty"
        |> Spec.lengthMax maxLength $"{propertyName} should not be longer than {maxLength} characters"
        |> Spec.createModel ShortString x

    let value (ShortString x) = x

type MediumString = private MediumString of string

module MediumString =
    [<Literal>]
    let private maxLength = 50

    let create propertyName x =
        Spec.def
        |> Spec.notNull $"{propertyName} should be specified"
        |> Spec.notEmpty $"{propertyName} value should not be empty"
        |> Spec.lengthMax maxLength $"{propertyName} should not be longer than {maxLength} characters"
        |> Spec.createModel MediumString x

    let value (MediumString x) = x

type LongString = private LongString of string

module LongString =
    [<Literal>]
    let private maxLength = 1000

    let create propertyName x =
        Spec.def
        |> Spec.notNull $"{propertyName} should be specified"
        |> Spec.notEmpty $"{propertyName} value should not be empty"
        |> Spec.lengthMax maxLength $"{propertyName} should not be longer than {maxLength} characters"
        |> Spec.createModel LongString x

    let value (LongString x) = x

type EmailAddress = private EmailAddress of string

module EmailAddress =
    let create propertyName x =
        Spec.def
        |> Spec.notNull $"{propertyName} should be specified"
        |> Spec.notEmpty $"{propertyName} should not be empty"
        |> Spec.createModel EmailAddress x

    let value (EmailAddress x) = x


type ServiceId = private ServiceId of System.Guid

module ServiceId =
    let newId () = ServiceId(System.Guid.NewGuid())
    let value (ServiceId x) = x

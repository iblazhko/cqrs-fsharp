namespace CQRS.Mapping

open System
open FPrimitive
open FsToolkit.ErrorHandling
open CQRS.Domain.Inventory
open CQRS.Domain.ValueTypes
open CQRS.EntityIds

module CountMapper =
    let fromDomain (domain: PositiveInteger) = domain |> PositiveInteger.value
    let toDomain (propName: string) (dto: int) = dto |> PositiveInteger.create propName

module StockQuantityMapper =
    let fromDomain (domain: StockQuantity) =
        match domain with
        | Empty -> 0
        | InventoryCount count -> count |> PositiveInteger.value

    let toDomain (propName: string) (n: int) : Result<StockQuantity, ErrorsByTag> =
        match n with
        | x when x < 0 -> Error(ErrorsByTag(Seq.singleton (propName, [ "Expected non-negative number" ])))
        | x when x = 0 -> Ok Empty
        | x -> x |> PositiveInteger.create propName |> Result.map InventoryCount

module InventoryIdMapper =
    let fromDomain (domain: InventoryId) =
        domain |> InventoryId.value |> EntityId.value

    let toDomain (propName: string) (dto: EntityIdRawValue) =
        dto |> EntityId.create propName |> Result.map InventoryId.create

module InventoryNameMapper =
    let fromDomain (domain: InventoryName) =
        domain |> InventoryName.value |> MediumString.value

    let toDomain (propName: string) (dto: string) =
        dto |> MediumString.create propName |> Result.map InventoryName.create

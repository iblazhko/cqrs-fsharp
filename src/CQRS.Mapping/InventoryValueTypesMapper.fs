namespace CQRS.Mapping

open FPrimitive
open FsToolkit.ErrorHandling
open CQRS.Domain.Inventory
open CQRS.Domain.ValueTypes
open CQRS.EntityIds

module Count' =
    let toDTO (domain: PositiveInteger) = domain |> PositiveInteger.value
    let ofDTO (propName: string) (dto: int) = dto |> PositiveInteger.create propName

module StockQuantity' =
    let toDTO (domain: StockQuantity) =
        match domain with
        | Empty -> 0
        | InventoryCount count -> count |> PositiveInteger.value

    let ofDTO (propName: string) (n: int) : Result<StockQuantity, ErrorsByTag> =
        match n with
        | 0 -> Ok Empty
        | x -> x |> PositiveInteger.create propName |> Result.map InventoryCount

module InventoryId' =
    let toDTO (domain: InventoryId) =
        domain |> InventoryId.value |> EntityId.value

    let ofDTO (propName: string) (dto: EntityIdRawValue) =
        dto |> EntityId.create propName |> Result.map InventoryId.create

module InventoryName' =
    let toDTO (domain: InventoryName) =
        domain |> InventoryName.value |> MediumString.value

    let ofDTO (propName: string) (dto: string) =
        dto |> MediumString.create propName |> Result.map InventoryName.create

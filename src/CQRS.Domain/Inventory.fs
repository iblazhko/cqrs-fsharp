module CQRS.Domain.Inventory

open FPrimitive
open CQRS.Domain.ValueTypes
open CQRS.EntityIds

type InventoryId = private InventoryId of EntityId

module InventoryId =
    [<Literal>]
    let private stringPrefix = "Inventory-"

    let newId () = InventoryId(EntityId.newId ())

    let create (id: EntityId) = InventoryId id

    let value (InventoryId id) = id

    let rec toString (InventoryId id) =
        $"{stringPrefix}{(id |> EntityId.toString)}"

    let fromString propertyName (s: string) =
        s.Substring(stringPrefix.Length)
        |> EntityId.fromString propertyName
        |> Result.map InventoryId

type InventoryName = private InventoryName of MediumString

module InventoryName =
    let create (n: MediumString) = InventoryName n
    let value (InventoryName n) = n

type StockQuantity =
    | Empty
    | InventoryCount of PositiveInteger

module StockQuantity =
    let create (n: PositiveInteger) = InventoryCount n

    let add (stockQuantity: StockQuantity) (n: PositiveInteger) =
        match stockQuantity with
        | Empty -> InventoryCount n
        | InventoryCount count -> InventoryCount(PositiveInteger.add count n)

    let equals (stockQuantity: StockQuantity) (n: PositiveInteger) =
        match stockQuantity with
        | Empty -> false
        | InventoryCount x -> x = n

    let subtract (stockQuantity: StockQuantity) (n: PositiveInteger) =
        match stockQuantity with
        | Empty -> failwith "Cannot subtract from Empty"
        | InventoryCount count ->
            if (equals stockQuantity n) then
                Empty
            else
                InventoryCount(PositiveInteger.subtract count n)

// State
(*
Instead of having IsNew and IsActive flags we could define distinct states like this
type InventoryState =
  | NewInventory of NewInventory
  | InactiveInventory of InactiveInventory
  | Inventory of Inventory

This would make aggregate cleaner, but OTOH it would make appliers more complicated
because we will not be able to use "{ with Property=NewValue }" syntax.
*)
type InventoryState =
    { InventoryId: InventoryId
      Name: InventoryName
      StockQuantity: StockQuantity
      IsNew: bool
      IsActive: bool }

// Domain commands

type CreateInventory =
    { InventoryId: InventoryId
      Name: InventoryName }

type RenameInventory =
    { InventoryId: InventoryId
      NewName: InventoryName }

type AddItemsToInventory =
    { InventoryId: InventoryId
      Count: PositiveInteger }

type RemoveItemsFromInventory =
    { InventoryId: InventoryId
      Count: PositiveInteger }

type DeactivateInventory = { InventoryId: InventoryId }

// Domain events

// Inventory Name is mentioned in every domain event
//
// In theory, we could remove this from *domain* events,
// this information can be inferred from the state.
//
// However Name is a useful information to have *in DTOs*,
// and not having it in the domain events can make
// DTO <-> domain event mapping more complicated.

type InventoryCreated =
    { InventoryId: InventoryId
      Name: InventoryName
      IsActive: bool }

type InventoryRenamed =
    { InventoryId: InventoryId
      OldName: InventoryName
      NewName: InventoryName }

type ItemsAddedToInventory =
    { InventoryId: InventoryId
      Name: InventoryName
      AddedCount: PositiveInteger
      OldStockQuantity: StockQuantity
      NewStockQuantity: StockQuantity }

type ItemsRemovedFromInventory =
    { InventoryId: InventoryId
      Name: InventoryName
      RemovedCount: PositiveInteger
      OldStockQuantity: StockQuantity
      NewStockQuantity: StockQuantity }

type ItemInStock =
    { InventoryId: InventoryId
      Name: InventoryName
      StockQuantity: StockQuantity }

type ItemWentOutOfStock =
    { InventoryId: InventoryId
      Name: InventoryName }

type RequestedMoreItemsThanHaveInStock =
    { InventoryId: InventoryId
      Name: InventoryName
      StockQuantity: StockQuantity
      RequestedCount: PositiveInteger }

type InventoryDeactivated =
    { InventoryId: InventoryId
      Name: InventoryName }

// All domain commands
type InventoryCommand =
    | CreateInventory of CreateInventory
    | RenameInventory of RenameInventory
    | AddItemsToInventory of AddItemsToInventory
    | RemoveItemsFromInventory of RemoveItemsFromInventory
    | DeactivateInventory of DeactivateInventory

// All domain events
type InventoryEvent =
    | InventoryCreated of InventoryCreated
    | InventoryRenamed of InventoryRenamed
    | ItemsAddedToInventory of ItemsAddedToInventory
    | ItemsRemovedFromInventory of ItemsRemovedFromInventory
    | ItemInStock of ItemInStock
    | ItemWentOutOfStock of ItemWentOutOfStock
    | RequestedMoreItemsThanHaveInStock of RequestedMoreItemsThanHaveInStock
    | InventoryDeactivated of InventoryDeactivated

// All domain validation failures
type ValidationFailure =
    | DoesNotExist of InventoryId
    | AlreadyExists of InventoryId
    | Deactivated of InventoryId
    | CannotDeactivateNonEmpty of InventoryId
    | CannotDeactivateWhenMoonIsFull of InventoryId
    | CannotRequestMoreThanHaveInStock of InventoryId
    | InventoryIdMismatch of InventoryId * InventoryId
    | ValidationError of ErrorsByTag

// All failures
type InventoryFailure = ValidationFailure of ValidationFailure

// Everything that can happen with Inventory
type DomainMessage =
    | InventoryCommand of InventoryCommand
    | InventoryEvent of InventoryEvent
    | InventoryFailure of InventoryFailure

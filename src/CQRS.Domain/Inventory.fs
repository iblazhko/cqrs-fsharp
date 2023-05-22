module CQRS.Domain.Inventory

open FPrimitive
open CQRS.Domain.ValueTypes
open CQRS.EntityIds

type InventoryItemId = private InventoryItemId of EntityId

module InventoryItemId =
    [<Literal>]
    let private stringPrefix = "InventoryItem-"

    let newId () = InventoryItemId(EntityId.newId ())

    let create (id: EntityId) = InventoryItemId id

    let value (InventoryItemId id) = id

    let rec toString (InventoryItemId id) =
        $"{stringPrefix}{(id |> EntityId.toString)}"

    let fromString propertyName (s: string) =
        s.Substring(stringPrefix.Length)
        |> EntityId.fromString propertyName
        |> Result.map InventoryItemId

type InventoryItemName = private InventoryItemName of MediumString

module InventoryItemName =
    let create (n: MediumString) = InventoryItemName n
    let value (InventoryItemName n) = n

type StockQuantity =
    | Empty
    | InventoryCount of PositiveInteger

module StockQuantity =
    let create (n: PositiveInteger) = InventoryCount n

    let add (stockQuantity: StockQuantity) (n: PositiveInteger) =
        match stockQuantity with
        | Empty -> InventoryCount n
        | InventoryCount count -> InventoryCount(PositiveInteger.add count n)

    let subtract (stockQuantity: StockQuantity) (n: PositiveInteger) =
        match stockQuantity with
        | Empty -> failwith "Cannot subtract from Empty"
        | InventoryCount count -> InventoryCount(PositiveInteger.subtract count n)

// State
(*
Instead of having IsNew and IsActive flags we could define distinct states like this
type InventoryItemState =
  | NewInventory of NewInventory
  | InactiveInventory of InactiveInventory
  | Inventory of Inventory

This would make aggregate cleaner, but OTOH it would make appliers more complicated
because we will not be able to use "{ with Property=NewValue }" syntax.
*)
type InventoryItemState =
    { InventoryItemId: InventoryItemId
      Name: InventoryItemName
      StockQuantity: StockQuantity
      IsNew: bool
      IsActive: bool }

// Domain commands

type CreateInventoryItem =
    { InventoryItemId: InventoryItemId
      Name: InventoryItemName }

type RenameInventoryItem =
    { InventoryItemId: InventoryItemId
      NewName: InventoryItemName }

type AddItemsToInventory =
    { InventoryItemId: InventoryItemId
      Count: PositiveInteger }

type RemoveItemsFromInventory =
    { InventoryItemId: InventoryItemId
      Count: PositiveInteger }

type DeactivateInventoryItem = { InventoryItemId: InventoryItemId }

// Domain events

type InventoryItemCreated =
    { InventoryItemId: InventoryItemId
      Name: InventoryItemName }

type InventoryItemRenamed =
    { InventoryItemId: InventoryItemId
      OldName: InventoryItemName
      NewName: InventoryItemName }

type ItemsAddedToInventory =
    { InventoryItemId: InventoryItemId
      Name: InventoryItemName
      AddedCount: PositiveInteger
      OldStockQuantity: StockQuantity
      NewStockQuantity: StockQuantity }

type ItemsRemovedFromInventory =
    { InventoryItemId: InventoryItemId
      Name: InventoryItemName
      RemovedCount: PositiveInteger
      OldStockQuantity: StockQuantity
      NewStockQuantity: StockQuantity }

type ItemNotInStock =
    { InventoryItemId: InventoryItemId
      Name: InventoryItemName }

type ItemWentOutOfStock =
    { InventoryItemId: InventoryItemId
      Name: InventoryItemName }

type InventoryItemDeactivated =
    { InventoryItemId: InventoryItemId
      Name: InventoryItemName }

// All domain commands
type InventoryItemCommand =
    | CreateInventoryItem of CreateInventoryItem
    | RenameInventoryItem of RenameInventoryItem
    | AddItemsToInventory of AddItemsToInventory
    | RemoveItemsFromInventory of RemoveItemsFromInventory
    | DeactivateInventoryItem of DeactivateInventoryItem

// All domain events
type InventoryItemEvent =
    | InventoryItemCreated of InventoryItemCreated
    | InventoryItemRenamed of InventoryItemRenamed
    | ItemsAddedToInventory of ItemsAddedToInventory
    | ItemsRemovedFromInventory of ItemsRemovedFromInventory
    | ItemNotInStock of ItemNotInStock
    | ItemWentOutOfStock of ItemWentOutOfStock
    | InventoryItemDeactivated of InventoryItemDeactivated

// All domain validation failures
type ValidationFailure =
    | DoesNotExist of InventoryItemId
    | AlreadyExists of InventoryItemId
    | Deactivated of InventoryItemId
    | CannotDeactivateNonEmpty of InventoryItemId
    | ValidationError of ErrorsByTag

// All failures
type InventoryFailure = ValidationFailure of ValidationFailure

// Everything that can happen with Inventory
type DomainMessage =
    | InventoryCommand of InventoryItemCommand
    | InventoryEvent of InventoryItemEvent
    | InventoryFailure of InventoryFailure

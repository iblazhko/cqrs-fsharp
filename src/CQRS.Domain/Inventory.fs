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

    let toString (InventoryId id) =
        $"{stringPrefix}{(id |> EntityId.value)}"

    let fromString propertyName (s: string) =
        s.Substring(stringPrefix.Length)
        |> EntityId.create propertyName
        |> Result.map InventoryId

type InventoryName = private InventoryName of MediumString

module InventoryName =
    let create (n: MediumString) = InventoryName n
    let value (InventoryName n) = n

type SubtractFailure =
    | CannotSubtractFromEmpty
    | SubtractValueOutOfRange of string

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
        | Empty -> Error(CannotSubtractFromEmpty)
        | InventoryCount count ->
            if (equals stockQuantity n) then
                Ok Empty
            else
                match PositiveInteger.subtract count n with
                | Ok x -> Ok(InventoryCount(x))
                | Error e -> Error(SubtractValueOutOfRange(e))

// State
type InventoryData =
    { InventoryId: InventoryId
      Name: InventoryName
      StockQuantity: StockQuantity }

type InventoryState =
    | Uninitialized of InventoryId
    | Active of InventoryData
    | Inactive of InventoryData

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
// However, Name is useful information to have *in DTOs*,
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

module InventoryCommand =
    let inventoryId (cmd: InventoryCommand) =
        match cmd with
        | CreateInventory x -> x.InventoryId
        | RenameInventory x -> x.InventoryId
        | AddItemsToInventory x -> x.InventoryId
        | RemoveItemsFromInventory x -> x.InventoryId
        | DeactivateInventory x -> x.InventoryId

// All domain events
type InventoryEvent =
    | InventoryCreated of InventoryCreated
    | InventoryRenamed of InventoryRenamed
    | ItemsAddedToInventory of ItemsAddedToInventory
    | ItemsRemovedFromInventory of ItemsRemovedFromInventory
    | ItemInStock of ItemInStock
    | ItemWentOutOfStock of ItemWentOutOfStock
    | InventoryDeactivated of InventoryDeactivated

module InventoryEvent =
    let inventoryId (evt: InventoryEvent) =
        match evt with
        | InventoryCreated x -> x.InventoryId
        | InventoryRenamed x -> x.InventoryId
        | ItemsAddedToInventory x -> x.InventoryId
        | ItemsRemovedFromInventory x -> x.InventoryId
        | ItemInStock x -> x.InventoryId
        | ItemWentOutOfStock x -> x.InventoryId
        | InventoryDeactivated x -> x.InventoryId

// All domain failures
type InventoryFailure =
    | DoesNotExist of InventoryId
    | AlreadyExists of InventoryId
    | Deactivated of InventoryId
    | CannotDeactivateNonEmpty of InventoryId
    | CannotDeactivateWhenMoonIsFull of InventoryId
    | CannotRequestMoreThanHaveInStock of InventoryId
    | InventoryIdMismatch of InventoryId * InventoryId
    | ValidationError of ErrorsByTag

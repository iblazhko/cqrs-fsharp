module CQRS.Domain.InventoryItemAggregate

open CQRS.Domain.ValueTypes
open CQRS.Domain.Inventory

let private invokeIfNew (state: InventoryItemState) (action: unit -> InventoryItemEvent seq) =
    match state with
    | x when not x.IsNew -> Error(ValidationFailure(AlreadyExists x.InventoryItemId))
    | x when not x.IsActive -> Error(ValidationFailure(Deactivated x.InventoryItemId))
    | _ -> Ok(action ())

let private invokeIfExists (state: InventoryItemState) (action: InventoryItemState -> InventoryItemEvent seq) =
    match state with
    | x when x.IsNew -> Error(ValidationFailure(DoesNotExist x.InventoryItemId))
    | x when not x.IsActive -> Error(ValidationFailure(Deactivated x.InventoryItemId))
    | x -> Ok(action x)

let private deactivateIfEmpty (state: InventoryItemState) (action: InventoryItemState -> InventoryItemEvent seq) =
    match state with
    | x when x.IsNew -> Error(ValidationFailure(DoesNotExist x.InventoryItemId))
    | x when not x.IsActive -> Error(ValidationFailure(Deactivated x.InventoryItemId))
    | x ->
        match x.StockQuantity with
        | StockQuantity.InventoryCount _ -> Error(ValidationFailure(CannotDeactivateNonEmpty x.InventoryItemId))
        | StockQuantity.Empty -> Ok(action x)

let private getNotInStockEvent (x: InventoryItemState) =
    ItemNotInStock
        { InventoryItemId = x.InventoryItemId
          Name = x.Name }

let private getWentOutOfStockEvent (x: InventoryItemState) =
    ItemWentOutOfStock
        { InventoryItemId = x.InventoryItemId
          Name = x.Name }

let private getRemovedFromInventoryEvent (x: InventoryItemState) removedCount =
    ItemsRemovedFromInventory
        { InventoryItemId = x.InventoryItemId
          Name = x.Name
          RemovedCount = removedCount
          OldStockQuantity = x.StockQuantity
          NewStockQuantity = StockQuantity.subtract x.StockQuantity removedCount }

let createWithId (state: InventoryItemState) (id: InventoryItemId) (name: InventoryItemName) =
    invokeIfNew state (fun () -> Seq.singleton (InventoryItemCreated { InventoryItemId = id; Name = name }))

let create (state: InventoryItemState) (name: InventoryItemName) =
    createWithId state (InventoryItemId.newId ()) name

let changeName (state: InventoryItemState) (newName: InventoryItemName) =
    invokeIfExists state (fun x ->
        seq {
            if not (state.Name = newName) then
                yield
                    InventoryItemRenamed
                        { InventoryItemId = x.InventoryItemId
                          OldName = x.Name
                          NewName = newName }
        })

let add (state: InventoryItemState) (count: PositiveInteger) =
    invokeIfExists state (fun x ->
        Seq.singleton (
            ItemsAddedToInventory
                { InventoryItemId = x.InventoryItemId
                  Name = x.Name
                  AddedCount = count
                  OldStockQuantity = x.StockQuantity
                  NewStockQuantity = StockQuantity.add x.StockQuantity count }
        ))


let remove (state: InventoryItemState) (count: PositiveInteger) =
    invokeIfExists state (fun x ->
        seq {
            match x.StockQuantity with
            | StockQuantity.Empty -> yield getNotInStockEvent x
            | StockQuantity.InventoryCount available ->
                match available with
                | amount when PositiveInteger.greaterThan amount count -> yield getRemovedFromInventoryEvent x count
                | amount when PositiveInteger.equal amount count ->
                    yield getRemovedFromInventoryEvent x count
                    yield getWentOutOfStockEvent x
                | _ -> yield getNotInStockEvent x
        })

let deactivate (state: InventoryItemState) =
    deactivateIfEmpty state (fun x ->
        Seq.singleton (
            InventoryItemDeactivated
                { InventoryItemId = x.InventoryItemId
                  Name = x.Name }
        ))

module CQRS.Domain.InventoryAggregate

open CQRS.Domain.ValueTypes
open CQRS.Domain.Inventory

let private invokeIfNew (state: InventoryState) (action: unit -> InventoryEvent seq) =
    match state with
    | x when not x.IsNew -> Error(ValidationFailure(AlreadyExists x.InventoryId))
    | x when not x.IsActive -> Error(ValidationFailure(Deactivated x.InventoryId))
    | _ -> Ok(action ())

let private invokeIfExists (state: InventoryState) (action: InventoryState -> InventoryEvent seq) =
    match state with
    | x when x.IsNew -> Error(ValidationFailure(DoesNotExist x.InventoryId))
    | x when not x.IsActive -> Error(ValidationFailure(Deactivated x.InventoryId))
    | x -> Ok(action x)

let private deactivateIfEmpty (state: InventoryState) (action: InventoryState -> InventoryEvent seq) =
    match state with
    | x when x.IsNew -> Error(ValidationFailure(DoesNotExist x.InventoryId))
    | x when not x.IsActive -> Error(ValidationFailure(Deactivated x.InventoryId))
    | x ->
        match x.StockQuantity with
        | StockQuantity.InventoryCount _ -> Error(ValidationFailure(CannotDeactivateNonEmpty x.InventoryId))
        | StockQuantity.Empty -> Ok(action x)

let private getNotInStockEvent (x: InventoryState) =
    ItemNotInStock
        { InventoryId = x.InventoryId
          Name = x.Name }

let private getWentOutOfStockEvent (x: InventoryState) =
    ItemWentOutOfStock
        { InventoryId = x.InventoryId
          Name = x.Name }

let private getRemovedFromInventoryEvent (x: InventoryState) removedCount =
    ItemsRemovedFromInventory
        { InventoryId = x.InventoryId
          Name = x.Name
          RemovedCount = removedCount
          OldStockQuantity = x.StockQuantity
          NewStockQuantity = StockQuantity.subtract x.StockQuantity removedCount }

let create (state: InventoryState) (id: InventoryId) (name: InventoryName) =
    invokeIfNew state (fun () ->
        Seq.singleton (
            InventoryCreated
                { InventoryId = id
                  Name = name
                  IsActive = true }
        ))

let rename (state: InventoryState) (newName: InventoryName) =
    invokeIfExists state (fun x ->
        seq {
            if not (state.Name = newName) then
                yield
                    InventoryRenamed
                        { InventoryId = x.InventoryId
                          OldName = x.Name
                          NewName = newName }
        })

let addItems (state: InventoryState) (count: PositiveInteger) =
    invokeIfExists state (fun x ->
        Seq.singleton (
            ItemsAddedToInventory
                { InventoryId = x.InventoryId
                  Name = x.Name
                  AddedCount = count
                  OldStockQuantity = x.StockQuantity
                  NewStockQuantity = StockQuantity.add x.StockQuantity count }
        ))


let removeItems (state: InventoryState) (count: PositiveInteger) =
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

let deactivate (state: InventoryState) =
    deactivateIfEmpty state (fun x ->
        Seq.singleton (
            InventoryDeactivated
                { InventoryId = x.InventoryId
                  Name = x.Name }
        ))

module CQRS.Domain.InventoryAggregate

open CQRS.Domain.Moon
open CQRS.Domain.ValueTypes
open CQRS.Domain.Inventory

let private invokeIfNew (state: InventoryState) (action: unit -> InventoryEvent seq) =
    match state with
    | x when not x.IsNew -> Error(ValidationFailure(AlreadyExists x.InventoryId))
    | x when not x.IsActive -> Error(ValidationFailure(Deactivated x.InventoryId))
    | _ -> Ok(action ())

let private invokeIfExists (state: InventoryState) (id: InventoryId) (action: InventoryState -> InventoryEvent seq) =
    match state with
    | x when x.IsNew -> Error(ValidationFailure(DoesNotExist x.InventoryId))
    | x when not x.IsActive -> Error(ValidationFailure(Deactivated x.InventoryId))
    | x when not (x.InventoryId = id) -> Error(ValidationFailure(InventoryIdMismatch(x.InventoryId, id)))
    | x -> Ok(action x)

let private deactivateIfEmpty (state: InventoryState) (id: InventoryId) (action: InventoryState -> InventoryEvent seq) =
    match state with
    | x when x.IsNew -> Error(ValidationFailure(DoesNotExist x.InventoryId))
    | x when not x.IsActive -> Error(ValidationFailure(Deactivated x.InventoryId))
    | x when not (x.InventoryId = id) -> Error(ValidationFailure(InventoryIdMismatch(x.InventoryId, id)))
    | x ->
        match x.StockQuantity with
        | StockQuantity.InventoryCount _ -> Error(ValidationFailure(CannotDeactivateNonEmpty x.InventoryId))
        | StockQuantity.Empty -> Ok(action x)

let private getInStockEvent (x: InventoryState) count =
    ItemInStock
        { InventoryId = x.InventoryId
          Name = x.Name
          StockQuantity = count |> StockQuantity.create }

let private getWentOutOfStockEvent (x: InventoryState) =
    ItemWentOutOfStock
        { InventoryId = x.InventoryId
          Name = x.Name }

let private getNotEnoughStockEvent (x: InventoryState) requestedCount =
    RequestedMoreItemsThanHaveInStock
        { InventoryId = x.InventoryId
          Name = x.Name
          StockQuantity = x.StockQuantity
          RequestedCount = requestedCount }

let private getRemovedFromInventoryEvent (x: InventoryState) removedCount =
    ItemsRemovedFromInventory
        { InventoryId = x.InventoryId
          Name = x.Name
          RemovedCount = removedCount
          OldStockQuantity = x.StockQuantity
          NewStockQuantity = StockQuantity.subtract x.StockQuantity removedCount }

let create (state: InventoryState) (cmd: CreateInventory) =
    invokeIfNew state (fun () ->
        Seq.singleton (
            InventoryCreated
                { InventoryId = cmd.InventoryId
                  Name = cmd.Name
                  IsActive = true }
        ))

let rename (state: InventoryState) (cmd: RenameInventory) =
    invokeIfExists state cmd.InventoryId (fun x ->
        seq {
            if not (state.Name = cmd.NewName) then
                yield
                    InventoryRenamed
                        { InventoryId = x.InventoryId
                          OldName = x.Name
                          NewName = cmd.NewName }
        })

let addItems (state: InventoryState) (cmd: AddItemsToInventory) =
    invokeIfExists state cmd.InventoryId (fun x ->
        seq {
            let count = cmd.Count

            yield
                ItemsAddedToInventory
                    { InventoryId = x.InventoryId
                      Name = x.Name
                      AddedCount = count
                      OldStockQuantity = x.StockQuantity
                      NewStockQuantity = StockQuantity.add x.StockQuantity count }

            if state.StockQuantity = Empty then
                yield getInStockEvent x cmd.Count
        })

let removeItems (state: InventoryState) (cmd: RemoveItemsFromInventory) =
    invokeIfExists state cmd.InventoryId (fun x ->
        seq {
            let count = cmd.Count

            match x.StockQuantity with
            | StockQuantity.Empty -> yield getNotEnoughStockEvent x count
            | StockQuantity.InventoryCount available ->
                match available with
                | amount when PositiveInteger.greaterThan amount count -> yield getRemovedFromInventoryEvent x count
                | amount when PositiveInteger.equal amount count ->
                    yield getRemovedFromInventoryEvent x count
                    yield getWentOutOfStockEvent x
                | _ -> yield getNotEnoughStockEvent x count
        })

// Random business rule: cannot deactivate an inventory when the moon is in full phase
let deactivate (state: InventoryState) (moonPhase: MoonPhase) (cmd: DeactivateInventory) =
    match moonPhase with
    | FullMoon -> Error(ValidationFailure(CannotDeactivateWhenMoonIsFull cmd.InventoryId))
    | _ ->
        deactivateIfEmpty state cmd.InventoryId (fun x ->
            seq {
                InventoryDeactivated
                    { InventoryId = x.InventoryId
                      Name = x.Name }
            })

module CQRS.Domain.InventoryAggregate

open CQRS.Domain.Moon
open CQRS.Domain.Inventory

let private invokeIfNew (state: InventoryState) (action: unit -> InventoryEvent seq) =
    match state with
    | x when not x.IsNew -> Error(AlreadyExists x.InventoryId)
    | x when not x.IsActive -> Error(Deactivated x.InventoryId)
    | _ -> Ok(action ())

let private invokeIfExists (state: InventoryState) (id: InventoryId) (action: InventoryState -> InventoryEvent seq) =
    match state with
    | x when x.IsNew -> Error(DoesNotExist x.InventoryId)
    | x when not x.IsActive -> Error(Deactivated x.InventoryId)
    | x when not (x.InventoryId = id) -> Error(InventoryIdMismatch(x.InventoryId, id))
    | x -> Ok(action x)

let private invokeAsResultIfExists
    (state: InventoryState)
    (id: InventoryId)
    (action: InventoryState -> Result<InventoryEvent seq, InventoryFailure>)
    =
    match state with
    | x when x.IsNew -> Error(DoesNotExist x.InventoryId)
    | x when not x.IsActive -> Error(Deactivated x.InventoryId)
    | x when not (x.InventoryId = id) -> Error(InventoryIdMismatch(x.InventoryId, id))
    | x -> action x

let private deactivateIfEmpty (state: InventoryState) (id: InventoryId) (action: InventoryState -> InventoryEvent seq) =
    match state with
    | x when x.IsNew -> Error(DoesNotExist x.InventoryId)
    | x when not x.IsActive -> Error(Deactivated x.InventoryId)
    | x when not (x.InventoryId = id) -> Error(InventoryIdMismatch(x.InventoryId, id))
    | x ->
        match x.StockQuantity with
        | StockQuantity.InventoryCount _ -> Error(CannotDeactivateNonEmpty x.InventoryId)
        | StockQuantity.Empty -> Ok(action x)

let create (state: InventoryState) (cmd: CreateInventory) =
    invokeIfNew state (fun () ->
        seq {
            InventoryCreated
                { InventoryId = cmd.InventoryId
                  Name = cmd.Name
                  IsActive = true }
        })

let rename (state: InventoryState) (cmd: RenameInventory) =
    invokeIfExists state cmd.InventoryId (fun x ->
        seq {
            if not (state.Name = cmd.NewName) then
                InventoryRenamed
                    { InventoryId = x.InventoryId
                      OldName = x.Name
                      NewName = cmd.NewName }
        })

let addItems (state: InventoryState) (cmd: AddItemsToInventory) =
    invokeIfExists state cmd.InventoryId (fun x ->
        let count = cmd.Count

        seq {
            yield
                ItemsAddedToInventory
                    { InventoryId = x.InventoryId
                      Name = x.Name
                      AddedCount = count
                      OldStockQuantity = x.StockQuantity
                      NewStockQuantity = StockQuantity.add x.StockQuantity count }

            if state.StockQuantity = Empty then
                yield
                    ItemInStock
                        { InventoryId = x.InventoryId
                          Name = x.Name
                          StockQuantity = count |> StockQuantity.create }
        })

let removeItems (state: InventoryState) (cmd: RemoveItemsFromInventory) =
    let getEvents (x: InventoryState) removedCount newQuantity =
        seq {
            yield
                ItemsRemovedFromInventory
                    { InventoryId = x.InventoryId
                      Name = x.Name
                      RemovedCount = removedCount
                      OldStockQuantity = x.StockQuantity
                      NewStockQuantity = newQuantity }

            if newQuantity = Empty then
                yield
                    ItemWentOutOfStock
                        { InventoryId = x.InventoryId
                          Name = x.Name }
        }

    invokeAsResultIfExists state cmd.InventoryId (fun x ->
        let removedCount = cmd.Count

        match StockQuantity.subtract x.StockQuantity removedCount with
        | Ok newQuantity ->  Ok(getEvents state removedCount newQuantity)
        | Error _ -> Error(CannotRequestMoreThanHaveInStock state.InventoryId))

// Random business rule: cannot deactivate an inventory when the moon is in full phase
let deactivate (state: InventoryState) (moonPhase: MoonPhase) (cmd: DeactivateInventory) =
    match moonPhase with
    | FullMoon -> Error(CannotDeactivateWhenMoonIsFull cmd.InventoryId)
    | _ ->
        deactivateIfEmpty state cmd.InventoryId (fun x ->
            seq {
                InventoryDeactivated
                    { InventoryId = x.InventoryId
                      Name = x.Name }
            })

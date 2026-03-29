module CQRS.Domain.InventoryAggregate

open CQRS.Domain.Moon
open CQRS.Domain.Inventory

let create (state: InventoryState) (cmd: CreateInventory) =
    match state with
    | Uninitialized _ ->
        Ok(seq {
            InventoryCreated
                { InventoryId = cmd.InventoryId
                  Name = cmd.Name
                  IsActive = true }
        })
    | Active data | Inactive data -> Error(AlreadyExists data.InventoryId)

let rename (state: InventoryState) (cmd: RenameInventory) =
    match state with
    | Uninitialized id -> Error(DoesNotExist id)
    | Inactive data -> Error(Deactivated data.InventoryId)
    | Active data when data.InventoryId <> cmd.InventoryId -> Error(InventoryIdMismatch(data.InventoryId, cmd.InventoryId))
    | Active data ->
        Ok(seq {
            if data.Name <> cmd.NewName then
                InventoryRenamed
                    { InventoryId = data.InventoryId
                      OldName = data.Name
                      NewName = cmd.NewName }
        })

let addItems (state: InventoryState) (cmd: AddItemsToInventory) =
    match state with
    | Uninitialized id -> Error(DoesNotExist id)
    | Inactive data -> Error(Deactivated data.InventoryId)
    | Active data when data.InventoryId <> cmd.InventoryId -> Error(InventoryIdMismatch(data.InventoryId, cmd.InventoryId))
    | Active data ->
        let count = cmd.Count

        Ok(seq {
            yield
                ItemsAddedToInventory
                    { InventoryId = data.InventoryId
                      Name = data.Name
                      AddedCount = count
                      OldStockQuantity = data.StockQuantity
                      NewStockQuantity = StockQuantity.add data.StockQuantity count }

            if data.StockQuantity = Empty then
                yield
                    ItemInStock
                        { InventoryId = data.InventoryId
                          Name = data.Name
                          StockQuantity = count |> StockQuantity.create }
        })

let removeItems (state: InventoryState) (cmd: RemoveItemsFromInventory) =
    match state with
    | Uninitialized id -> Error(DoesNotExist id)
    | Inactive data -> Error(Deactivated data.InventoryId)
    | Active data when data.InventoryId <> cmd.InventoryId -> Error(InventoryIdMismatch(data.InventoryId, cmd.InventoryId))
    | Active data ->
        let removedCount = cmd.Count

        let getEvents newQuantity =
            seq {
                yield
                    ItemsRemovedFromInventory
                        { InventoryId = data.InventoryId
                          Name = data.Name
                          RemovedCount = removedCount
                          OldStockQuantity = data.StockQuantity
                          NewStockQuantity = newQuantity }

                if newQuantity = Empty then
                    yield
                        ItemWentOutOfStock
                            { InventoryId = data.InventoryId
                              Name = data.Name }
            }

        match StockQuantity.subtract data.StockQuantity removedCount with
        | Ok newQuantity -> Ok(getEvents newQuantity)
        | Error _ -> Error(CannotRequestMoreThanHaveInStock data.InventoryId)

// Random business rule: cannot deactivate an inventory when the moon is in full phase
let deactivate (state: InventoryState) (moonPhase: MoonPhase) (cmd: DeactivateInventory) =
    match moonPhase with
    | FullMoon -> Error(CannotDeactivateWhenMoonIsFull cmd.InventoryId)
    | _ ->
        match state with
        | Uninitialized id -> Error(DoesNotExist id)
        | Inactive data -> Error(Deactivated data.InventoryId)
        | Active data when data.InventoryId <> cmd.InventoryId -> Error(InventoryIdMismatch(data.InventoryId, cmd.InventoryId))
        | Active data ->
            match data.StockQuantity with
            | InventoryCount _ -> Error(CannotDeactivateNonEmpty data.InventoryId)
            | Empty ->
                Ok(seq {
                    InventoryDeactivated
                        { InventoryId = data.InventoryId
                          Name = data.Name }
                })

module CQRS.Domain.InventoryEventsApplier

open CQRS.Domain.Inventory

let apply (state: InventoryState) (e: InventoryEvent) : InventoryState =
    match e with
    | InventoryCreated x ->
        { state with
            InventoryId = x.InventoryId
            Name = x.Name
            StockQuantity = Empty
            IsNew = false
            IsActive = true }
    | InventoryDeactivated _ -> { state with IsActive = false }
    | InventoryRenamed x -> { state with Name = x.NewName }
    | ItemsAddedToInventory x ->
        { state with
            StockQuantity = x.NewStockQuantity }
    | ItemsRemovedFromInventory x ->
        { state with
            StockQuantity = x.NewStockQuantity }
    | ItemNotInStock _ -> state
    | ItemWentOutOfStock _ -> state

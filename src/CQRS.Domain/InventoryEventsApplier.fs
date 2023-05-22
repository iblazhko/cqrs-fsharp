module CQRS.Domain.InventoryEventsApplier

open CQRS.Domain.Inventory

let apply (state: InventoryItemState) (e: InventoryItemEvent) : InventoryItemState =
    match e with
    | InventoryItemCreated x ->
        { state with
            InventoryItemId = x.InventoryItemId
            Name = x.Name
            StockQuantity = Empty
            IsNew = false
            IsActive = true }
    | InventoryItemDeactivated x -> { state with IsActive = false }
    | InventoryItemRenamed x -> { state with Name = x.NewName }
    | ItemsAddedToInventory x ->
        { state with
            StockQuantity = x.NewStockQuantity }
    | ItemsRemovedFromInventory x ->
        { state with
            StockQuantity = x.NewStockQuantity }
    | ItemNotInStock _ -> state
    | ItemWentOutOfStock _ -> state

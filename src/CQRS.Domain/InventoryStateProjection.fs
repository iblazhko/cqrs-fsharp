module CQRS.Domain.InventoryStateProjection

open CQRS.Domain.Inventory

let apply (state: InventoryState) (e: InventoryEvent) : InventoryState =
    match e with
    | InventoryCreated x ->
        { state with
            Name = x.Name
            StockQuantity = Empty
            IsActive = x.IsActive
            IsNew = false }
    | InventoryDeactivated _ -> { state with IsActive = false }
    | InventoryRenamed x -> { state with Name = x.NewName }
    | ItemsAddedToInventory x ->
        { state with
            StockQuantity = x.NewStockQuantity }
    | ItemsRemovedFromInventory x ->
        { state with
            StockQuantity = x.NewStockQuantity }
    | ItemInStock _ -> state
    | ItemWentOutOfStock _ -> state

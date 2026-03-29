module CQRS.Domain.InventoryStateProjection

open CQRS.Domain.Inventory

let apply (state: InventoryState) (e: InventoryEvent) : InventoryState =
    match e with
    | InventoryCreated x ->
        let data: InventoryData = { InventoryId = x.InventoryId; Name = x.Name; StockQuantity = Empty }
        if x.IsActive then Active data else Inactive data
    | InventoryDeactivated _ ->
        match state with
        | Active data -> Inactive data
        | _ -> state
    | InventoryRenamed x ->
        match state with
        | Active data -> Active { data with Name = x.NewName }
        | _ -> state
    | ItemsAddedToInventory x ->
        match state with
        | Active data -> Active { data with StockQuantity = x.NewStockQuantity }
        | _ -> state
    | ItemsRemovedFromInventory x ->
        match state with
        | Active data -> Active { data with StockQuantity = x.NewStockQuantity }
        | _ -> state
    | ItemInStock _ | ItemWentOutOfStock _ -> state

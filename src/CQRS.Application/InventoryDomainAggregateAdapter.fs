module CQRS.Application.InventoryDomainAggregateAdapter

open CQRS.Domain
open CQRS.Domain.Inventory
open FPrimitive
open FsToolkit.ErrorHandling

(*
action InventoryState -> InventoryCommand -> Result<seq<InventoryEvent>,ErrorsByTag>

invokes domain aggregate methods and maps InventoryFailure to generic ErrorsByTag
*)

let create (currentState: InventoryState) (command: CreateInventory) =
    result {
        let! newEvents =
            InventoryAggregate.create currentState command.InventoryId command.Name
            |> Result.mapError (DomainErrorMapper.mapDomainError (nameof CreateInventory))

        return newEvents
    }

let rename (currentState: InventoryState) (command: RenameInventory) =
    result {
        let! newEvents =
            InventoryAggregate.rename currentState command.NewName
            |> Result.mapError (DomainErrorMapper.mapDomainError (nameof RenameInventory))

        return newEvents
    }

let addItemsToInventory (currentState: InventoryState) (command: AddItemsToInventory) =
    result {
        let! newEvents =
            InventoryAggregate.addItems currentState command.Count
            |> Result.mapError (DomainErrorMapper.mapDomainError (nameof RenameInventory))

        return newEvents
    }

let removeItemsFromInventory (currentState: InventoryState) (command: RemoveItemsFromInventory) =
    result {
        let! newEvents =
            InventoryAggregate.removeItems currentState command.Count
            |> Result.mapError (DomainErrorMapper.mapDomainError (nameof RenameInventory))

        return newEvents
    }

let deactivate (currentState: InventoryState) (_: DeactivateInventory) =
    result {
        let! newEvents =
            InventoryAggregate.deactivate currentState
            |> Result.mapError (DomainErrorMapper.mapDomainError (nameof DeactivateInventory))

        return newEvents
    }

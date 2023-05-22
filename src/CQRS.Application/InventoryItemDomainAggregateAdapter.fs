module CQRS.Application.InventoryItemDomainAggregateAdapter

open CQRS.Domain
open CQRS.Domain.Inventory
open FPrimitive
open FsToolkit.ErrorHandling

(*
action InventoryItemState -> InventoryItemCommand -> Result<seq<InventoryItemEvent>,ErrorsByTag>

invokes domain aggregate methods and maps InventoryItemFailure to generic ErrorsByTag
*)

let create (currentState: InventoryItemState) (command: CreateInventoryItem) =
    result {
        let! newEvents =
            InventoryItemAggregate.create currentState command.InventoryItemId command.Name
            |> Result.mapError (DomainErrorMapper.mapDomainError (nameof CreateInventoryItem))

        return newEvents
    }

let rename (currentState: InventoryItemState) (command: RenameInventoryItem) =
    result {
        let! newEvents =
            InventoryItemAggregate.changeName currentState command.NewName
            |> Result.mapError (DomainErrorMapper.mapDomainError (nameof RenameInventoryItem))

        return newEvents
    }

let deactivate (currentState: InventoryItemState) (_: DeactivateInventoryItem) =
    result {
        let! newEvents =
            InventoryItemAggregate.deactivate currentState
            |> Result.mapError (DomainErrorMapper.mapDomainError (nameof DeactivateInventoryItem))

        return newEvents
    }

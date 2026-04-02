namespace CQRS.Application.WolverineConsumers

open CQRS.Application
open CQRS.DTO.V1
open Wolverine

(*
Wolverine discovers handlers by convention: any public method named Handle or HandleAsync
on a public class is treated as a message handler.
For commands, we need a separate queue per command type, matching our routing conventions
(see `CQRS.Infrastructure.WolverineConventions`). Separate handler classes ensure Wolverine
creates one queue per command type when using conventional routing.
*)

type CreateInventoryCommandConsumer(env: ApplicationEnvironment) =
    member _.HandleAsync(message: CreateInventoryCommand, envelope: Envelope) =
        WolverineDtoConsumer.handleCommand env message envelope

type RenameInventoryCommandConsumer(env: ApplicationEnvironment) =
    member _.HandleAsync(message: RenameInventoryCommand, envelope: Envelope) =
        WolverineDtoConsumer.handleCommand env message envelope

type AddItemsToInventoryCommandConsumer(env: ApplicationEnvironment) =
    member _.HandleAsync(message: AddItemsToInventoryCommand, envelope: Envelope) =
        WolverineDtoConsumer.handleCommand env message envelope

type RemoveItemsFromInventoryCommandConsumer(env: ApplicationEnvironment) =
    member _.HandleAsync(message: RemoveItemsFromInventoryCommand, envelope: Envelope) =
        WolverineDtoConsumer.handleCommand env message envelope

type DeactivateInventoryCommandConsumer(env: ApplicationEnvironment) =
    member _.HandleAsync(message: DeactivateInventoryCommand, envelope: Envelope) =
        WolverineDtoConsumer.handleCommand env message envelope

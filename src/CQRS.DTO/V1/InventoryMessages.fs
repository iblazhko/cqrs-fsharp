namespace CQRS.DTO.V1

open System
open CQRS.DTO
open CQRS.EntityIds

// DTOs are meant to be interoperable with outside world
// hence using nullable classes with parameterless constructors
// and get/set properties that can be nullable

// Commands

[<AllowNullLiteral>]
type CreateInventoryCommand() =
    interface CqrsCommandDto
    member val InventoryId: EntityIdRawValue = EntityIdRawValue.Empty with get, set
    member val Name: string = String.Empty with get, set
    override this.ToString() = Json.serialize this

[<AllowNullLiteral>]
type RenameInventoryCommand() =
    interface CqrsCommandDto
    member val InventoryId: EntityIdRawValue = EntityIdRawValue.Empty with get, set
    member val NewName: string = String.Empty with get, set
    override this.ToString() = Json.serialize this

[<AllowNullLiteral>]
type AddItemsToInventoryCommand() =
    interface CqrsCommandDto
    member val InventoryId: EntityIdRawValue = EntityIdRawValue.Empty with get, set
    member val Count: int = 0 with get, set
    override this.ToString() = Json.serialize this

[<AllowNullLiteral>]
type RemoveItemsFromInventoryCommand() =
    interface CqrsCommandDto
    member val InventoryId: EntityIdRawValue = EntityIdRawValue.Empty with get, set
    member val Count: int = 0 with get, set
    override this.ToString() = Json.serialize this


[<AllowNullLiteral>]
type DeactivateInventoryCommand() =
    interface CqrsCommandDto
    member val InventoryId: EntityIdRawValue = EntityIdRawValue.Empty with get, set
    override this.ToString() = Json.serialize this

// Events

[<AllowNullLiteral>]
type InventoryCreatedEvent() =
    interface CqrsEventDto
    member val InventoryId: EntityIdRawValue = EntityIdRawValue.Empty with get, set
    member val Name: string = String.Empty with get, set
    member val IsActive: bool = false with get, set
    override this.ToString() = Json.serialize this

[<AllowNullLiteral>]
type InventoryRenamedEvent() =
    interface CqrsEventDto
    member val InventoryId: EntityIdRawValue = EntityIdRawValue.Empty with get, set
    member val OldName: string = String.Empty with get, set
    member val NewName: string = String.Empty with get, set
    override this.ToString() = Json.serialize this

[<AllowNullLiteral>]
type ItemsAddedToInventoryEvent() =
    interface CqrsEventDto
    member val InventoryId: EntityIdRawValue = EntityIdRawValue.Empty with get, set
    member val Name: string = String.Empty with get, set
    member val AddedCount: int = 0 with get, set
    member val OldStockQuantity: int = 0 with get, set
    member val NewStockQuantity: int = 0 with get, set
    override this.ToString() = Json.serialize this

[<AllowNullLiteral>]
type ItemsRemovedFromInventoryEvent() =
    interface CqrsEventDto
    member val InventoryId: EntityIdRawValue = EntityIdRawValue.Empty with get, set
    member val Name: string = String.Empty with get, set
    member val RemovedCount: int = 0 with get, set
    member val OldStockQuantity: int = 0 with get, set
    member val NewStockQuantity: int = 0 with get, set
    override this.ToString() = Json.serialize this

[<AllowNullLiteral>]
type ItemNotInStockEvent() =
    interface CqrsEventDto
    member val InventoryId: EntityIdRawValue = EntityIdRawValue.Empty with get, set
    member val Name: string = String.Empty with get, set
    override this.ToString() = Json.serialize this

[<AllowNullLiteral>]
type ItemWentOutOfStockEvent() =
    interface CqrsEventDto
    member val InventoryId: EntityIdRawValue = EntityIdRawValue.Empty with get, set
    member val Name: string = String.Empty with get, set
    override this.ToString() = Json.serialize this

[<AllowNullLiteral>]
type InventoryDeactivatedEvent() =
    interface CqrsEventDto
    member val InventoryId: EntityIdRawValue = EntityIdRawValue.Empty with get, set
    member val Name: string = String.Empty with get, set
    override this.ToString() = Json.serialize this

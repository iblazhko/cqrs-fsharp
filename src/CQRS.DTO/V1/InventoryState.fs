namespace CQRS.DTO.V1

open System
open CQRS.DTO
open CQRS.EntityIds

// This particular solution does not use snapshots caching
// when working with event streams, however it is possible
// in principle, and in that case this class would represent
// serialized shape of a snapshot.

[<AllowNullLiteral>]
type Inventory() =
    interface CqrsStateDto
    member val InventoryId: EntityIdRawValue = EntityIdRawValue.Empty with get, set
    member val Name: string = String.Empty with get, set
    member val StockQuantity: int = 0 with get, set
    member val IsNew: bool = false with get, set
    member val IsActive: bool = false with get, set
    override this.ToString() = Json.serialize this

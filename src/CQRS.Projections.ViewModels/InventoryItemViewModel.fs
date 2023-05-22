namespace CQRS.Projections

open System

// Projection view models are similar to messages DTOs, they are
// meant to be interoperable with outside world, hence using
// classes with parameterless constructors and get/set properties

[<AllowNullLiteral>]
type InventoryItemViewModel() =
    member val InventoryItemId: Guid = Guid.Empty with get, set
    member val Name: string = String.Empty with get, set
    member val StockCount: int = 0 with get, set
    member val IsActive: bool = false with get, set

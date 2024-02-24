namespace CQRS.Projections

open System

// Projection view models are similar to messages DTOs, they are
// meant to be interoperable with outside world, hence using
// classes with parameterless constructors and get/set properties

// In theory, we could have reused internal state DTO here
// (CQRS.DTO.V1.Inventory) - in this particular solution
// shapes of that DTO and this view model are identical.
// However in a real life project this most likely will not be
// the case, and we will need a dedicated view model specific for
// each particular read projection that is driven by a use case.
[<AllowNullLiteral>]
type InventoryViewModel() =
    member val InventoryId: string = String.Empty with get, set
    member val Name: string = String.Empty with get, set
    member val StockQuantity: int = 0 with get, set
    member val IsActive: bool = false with get, set

module CQRS.Projections.InventoryEventDtoHandler

open System.Threading.Tasks
open CQRS.DTO
open CQRS.Domain.Inventory
open CQRS.Mapping
open CQRS.Ports.ProjectionStore
open FPrimitive
open FsToolkit.ErrorHandling

// Unlike the Application where we have single "default" shape of state
// and single event stream state projection, here we may have multiple
// view models and multiple projections.
//
// This context structure is not an idiomatic FP w.r.t. dependencies handling,
// but it is good enough for the purpose of this solution, and this component is
// not in the Domain where this approach probably would not be acceptable.
type DomainEventDtoHandlerContext<'TEventDto, 'TViewModel when 'TEventDto :> CqrsDto and 'TViewModel: null> =
    { ProjectionStore: IProjectionStore<'TViewModel>
      DocumentCollectionIdFromEvent: InventoryEvent -> DocumentCollectionId
      DocumentIdFromEvent: InventoryEvent -> DocumentId
      ViewModelUpdateAction: InventoryEvent -> 'TViewModel -> 'TViewModel }

exception EventDtoMappingException of ErrorsByTag

let handleEvent<'TEventDto, 'TViewModel when 'TEventDto :> CqrsEventDto and 'TViewModel: null>
    (context: DomainEventDtoHandlerContext<'TEventDto, 'TViewModel>)
    (dto: 'TEventDto)
    : Task =
    task {
        let evt =
            dto
            |> InventoryEvent'.ofDTO
            |> Result.defaultWith (fun e -> raise (EventDtoMappingException e))

        let documentCollectionId = evt |> context.DocumentCollectionIdFromEvent
        let documentId = evt |> context.DocumentIdFromEvent

        use! documentCollection = context.ProjectionStore.OpenDocumentCollection<'TViewModel>(documentCollectionId)
        do! documentCollection.Update(documentId, context.ViewModelUpdateAction evt)
    }

module CQRS.Projections.InventoryEventDtoHandler

open System.Threading.Tasks
open CQRS.DTO
open CQRS.Ports.ProjectionStore
open FPrimitive
open Serilog

exception EventDtoMappingException of ErrorsByTag

// Unlike the Application where we have single "default" shape of state
// and single event stream state projection, here we may have multiple
// view models and multiple projections.
//
// This context structure is not an idiomatic FP w.r.t. dependencies handling,
// but it is good enough for the purpose of this solution, and this component is
// not in the Domain where this approach probably would not be acceptable.
type DomainEventDtoHandlerContext<'TEvent, 'TViewModel when 'TViewModel: null> =
    { ProjectionStore: IProjectionStore<'TViewModel>
      EventFromDto: CqrsEventDto -> Result<'TEvent, ErrorsByTag>
      DocumentCollectionIdFromEvent: 'TEvent -> DocumentCollectionId
      DocumentIdFromEvent: 'TEvent -> DocumentId
      ViewModelUpdateAction: 'TEvent -> 'TViewModel -> 'TViewModel }

let handleEvent<'TEventDto, 'TEvent, 'TViewModel when 'TEventDto :> CqrsEventDto and 'TViewModel: null>
    (context: DomainEventDtoHandlerContext<'TEvent, 'TViewModel>)
    (dto: 'TEventDto)
    : Task =
    task {
        match (dto :> CqrsEventDto) |> context.EventFromDto with
        | Error e ->
            Log.Logger.Warning(
                "[PROJECTION] Unmappable event DTO {DtoType}: {Error}",
                dto.GetType().FullName,
                e)
            raise (EventDtoMappingException e)
        | Ok evt ->
            let documentCollectionId = evt |> context.DocumentCollectionIdFromEvent
            let documentId = evt |> context.DocumentIdFromEvent
            use! documentCollection = context.ProjectionStore.OpenDocumentCollection<'TViewModel>(documentCollectionId)
            do! documentCollection.Update(documentId, context.ViewModelUpdateAction evt)
    }

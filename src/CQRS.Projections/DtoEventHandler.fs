module CQRS.Projections.DtoEventHandler

open System.Threading.Tasks
open CQRS.Ports.ProjectionStore
open FPrimitive
open FsToolkit.ErrorHandling

type internal DomainEventHandlerContext<'TEventDto, 'TEvent, 'TViewModel> =
    { DtoMapper: 'TEventDto -> Result<'TEvent, ErrorsByTag>
      DocumentCollectionIdFromEvent: 'TEvent -> DocumentCollectionId
      DocumentIdFromEvent: 'TEvent -> DocumentId
      ViewModelUpdateAction: 'TEvent -> 'TViewModel -> 'TViewModel
      DocumentStore: IDocumentStore<'TViewModel> }

exception EventHandlerException of ErrorsByTag

module internal DtoEventHandler =
    let handleEvent<'TEventDto, 'TEvent, 'TViewModel>
        (context: DomainEventHandlerContext<'TEventDto, 'TEvent, 'TViewModel>)
        (dto: 'TEventDto)
        : Task =
        task {
            let evt =
                dto
                |> context.DtoMapper
                |> Result.defaultWith (fun e -> raise (EventHandlerException e))

            let documentCollectionId = evt |> context.DocumentCollectionIdFromEvent
            let documentId = evt |> context.DocumentIdFromEvent

            let! documentCollection = context.DocumentStore.OpenDocumentCollection<'TViewModel>(documentCollectionId)
            do! documentCollection.Update(documentId, (context.ViewModelUpdateAction evt))

            return ()
        }

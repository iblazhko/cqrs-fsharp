module CQRS.Projections.DtoEventHandler

open System.Threading.Tasks
open CQRS.Ports.ProjectionStore
open FPrimitive
open FsToolkit.ErrorHandling

type internal DomainEventHandlerContext<'TEventDto, 'TEvent, 'TViewModel when 'TViewModel: null> =
    { DtoMapper: 'TEventDto -> Result<'TEvent, ErrorsByTag>
      DocumentCollectionIdFromEvent: 'TEvent -> DocumentCollectionId
      DocumentIdFromEvent: 'TEvent -> DocumentId
      ViewModelUpdateAction: 'TEvent -> 'TViewModel -> 'TViewModel
      ProjectionStore: IProjectionStore<'TViewModel> }

exception EventHandlerException of ErrorsByTag

module internal DtoEventHandler =
    let handleEvent<'TEventDto, 'TEvent, 'TViewModel when 'TViewModel: null>
        (context: DomainEventHandlerContext<'TEventDto, 'TEvent, 'TViewModel>)
        (dto: 'TEventDto)
        : Task = // TODO: Use Result<Task,EventHandlerFault>
        task {
            let evt =
                dto
                |> context.DtoMapper
                |> Result.defaultWith (fun e -> raise (EventHandlerException e))
            // TODO: map error to EventHandlerFault

            let documentCollectionId = evt |> context.DocumentCollectionIdFromEvent
            let documentId = evt |> context.DocumentIdFromEvent

            use! documentCollection = context.ProjectionStore.OpenDocumentCollection<'TViewModel>(documentCollectionId)
            do! documentCollection.Update(documentId, (context.ViewModelUpdateAction evt))
        // TODO: map ProjectionStore errors to EventHandlerFault
        }

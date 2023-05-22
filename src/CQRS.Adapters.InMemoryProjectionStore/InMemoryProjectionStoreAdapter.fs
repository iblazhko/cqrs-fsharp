namespace CQRS.Adapters

open System
open System.Collections.Concurrent
open System.Threading.Tasks
open CQRS.Ports.ProjectionStore

[<Sealed>]
type InMemoryDocumentCollection<'TViewModel>() =
    let docType = typedefof<'TViewModel>

    let newVm () =
        Activator.CreateInstance(docType) :?> 'TViewModel

    let documents = ConcurrentDictionary<DocumentId, 'TViewModel>()

    interface IDocumentCollection<'TViewModel> with
        member this.GetById(documentId) =
            task {
                let doc =
                    documents.GetOrAdd(documentId, (fun _ -> Activator.CreateInstance(docType) :?> 'TViewModel))

                return doc
            }

        member this.Update(documentId: DocumentId, updateAction: 'TViewModel -> 'TViewModel) : Task =
            task {
                documents.AddOrUpdate(
                    documentId,
                    (fun _ -> (newVm ()) |> updateAction),
                    (fun _ -> (fun vm -> vm |> updateAction))
                )
                |> ignore

                return ()
            }

        member this.Update(documentId: DocumentId, viewModel: 'TViewModel) : Task =
            task {
                documents.AddOrUpdate(documentId, (fun _ -> viewModel), (fun _ -> (fun _ -> viewModel)))
                |> ignore

                return ()
            }

        member this.Dispose() = ()
        member this.DisposeAsync() = ValueTask.CompletedTask

[<Sealed>]
type InMemoryProjectionStore<'TViewModel>() =
    let documentCollections =
        ConcurrentDictionary<DocumentCollectionId, InMemoryDocumentCollection<'TViewModel>>()

    interface IDocumentStore<'TViewModel> with
        member this.OpenDocumentCollection<'TViewModel>(documentCollectionId) =
            task {
                let collection =
                    documentCollections.GetOrAdd(
                        documentCollectionId,
                        (fun _ -> new InMemoryDocumentCollection<'TViewModel>())
                    )

                return collection
            }

        member this.Dispose() = ()
        member this.DisposeAsync() = ValueTask.CompletedTask

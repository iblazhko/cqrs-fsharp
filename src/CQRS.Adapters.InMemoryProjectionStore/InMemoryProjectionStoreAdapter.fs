namespace CQRS.Adapters.ProjectionStore

open System
open System.Collections.Concurrent
open System.Threading.Tasks
open CQRS.Ports.ProjectionStore
open Serilog

(*
Note that that this solution has two separate hosts: Application.Host and API.Host;
these hosts have their own instances of adapters, therefore when using
InMemoryProjectionStore adapter, documents stored in Application will not be visible in API.

This adapter is only suitable for unit or behavioural tests where everything
is hosted in the same process.
*)

[<Sealed>]
type InMemoryDocumentCollection<'TViewModel when 'TViewModel: null>() =
    let docType = typedefof<'TViewModel>

    let newVm () =
        Activator.CreateInstance(docType) :?> 'TViewModel

    let documents = ConcurrentDictionary<DocumentId, 'TViewModel>()

    interface IProjectionDocumentCollection<'TViewModel> with
        member this.GetById(documentId) =
            task {
                // TODO: Use explicit dependency for logging
                Log.Logger.Information("[PROJECTION] Retrieving {DocumentId}", documentId)

                let result =
                    match documents.TryGetValue(documentId) with
                    | true, doc -> Some(doc)
                    | false, _ -> None

                return result
            }

        member this.Update(documentId: DocumentId, updateAction: 'TViewModel -> 'TViewModel) : Task =
            task {
                // TODO: Use explicit dependency for logging
                Log.Logger.Information("[PROJECTION] Storing {DocumentId}", documentId)

                documents.AddOrUpdate(
                    documentId,
                    (fun _ -> (newVm ()) |> updateAction),
                    (fun _ -> (fun vm -> vm |> updateAction))
                )
                |> ignore
            }

        member this.Update(documentId: DocumentId, viewModel: 'TViewModel) : Task =
            task {
                // TODO: Use explicit dependency for logging
                Log.Logger.Information("[PROJECTION] Storing {DocumentId} {@Document}", documentId, viewModel)

                documents.AddOrUpdate(documentId, (fun _ -> viewModel), (fun _ -> (fun _ -> viewModel)))
                |> ignore
            }

        member this.Dispose() = ()
        member this.DisposeAsync() = ValueTask.CompletedTask

[<Sealed>]
type InMemoryProjectionStore<'TViewModel when 'TViewModel: null>() =
    let documentCollections =
        ConcurrentDictionary<DocumentCollectionId, InMemoryDocumentCollection<'TViewModel>>()

    interface IProjectionStore<'TViewModel> with
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

namespace CQRS.Adapters.ProjectionStore

open System
open System.Threading.Tasks
open CQRS.Ports.ProjectionStore
open Marten
open Serilog

[<Sealed>]
type MartenDbDocumentCollection<'TViewModel when 'TViewModel: null>(documentStore: IDocumentStore) =
    let session = documentStore.LightweightSession()

    let docType = typedefof<'TViewModel>

    let newVm () =
        Activator.CreateInstance(docType) :?> 'TViewModel

    let tryGetEnvelopeByDocumentId (s: IDocumentSession) documentId =
        task {
            let! envelope =
                s
                    .Query<DocumentEnvelope<'TViewModel>>()
                    .SingleOrDefaultAsync(fun (envelope: DocumentEnvelope<'TViewModel>) -> envelope.Id = documentId)

            let envelopeOption =
                match envelope with
                | e when (isNull e) -> None
                | e -> Some(e)

            return envelopeOption
        }

    interface IProjectionDocumentCollection<'TViewModel> with
        member this.GetById(documentId) =
            task {
                Log.Logger.Information("[PROJECTION] Retrieving {DocumentId}", documentId)

                let! envelopeOption = documentId |> tryGetEnvelopeByDocumentId session
                let result = envelopeOption |> Option.map (fun e -> e.VM)

                return result
            }

        member this.Update(documentId: DocumentId, updateAction: 'TViewModel -> 'TViewModel) : Task =
            task {
                Log.Logger.Information("[PROJECTION] Storing {DocumentId}", documentId)

                let! envelopeOption = documentId |> tryGetEnvelopeByDocumentId session

                let envelope =
                    match envelopeOption with
                    | Some e -> e
                    | None ->
                        let newEnvelope = DocumentEnvelope<'TViewModel>()
                        newEnvelope.Id <- documentId
                        newEnvelope.Version <- DocumentVersion.New
                        newEnvelope.VM <- newVm ()
                        newEnvelope

                let updatedVm = envelope.VM |> updateAction
                // TODO: consider checking if updateAction has actually made any changes

                envelope.VM <- updatedVm
                envelope.Version <- envelope.Version |> DocumentVersion.increment

                session.Store(envelope)
                do! session.SaveChangesAsync()
            }

        member this.Update(documentId: DocumentId, viewModel: 'TViewModel) : Task =
            task {
                Log.Logger.Information("[PROJECTION] Storing {DocumentId}", documentId)

                let! envelopeOption = documentId |> tryGetEnvelopeByDocumentId session

                let envelope =
                    match envelopeOption with
                    | Some e -> e
                    | None ->
                        let newEnvelope = DocumentEnvelope<'TViewModel>()
                        newEnvelope.Id <- documentId
                        newEnvelope.Version <- DocumentVersion.New
                        // no need to initialize newEnvelope.VM - it will be updated below
                        newEnvelope

                // TODO: consider checking if viewModel has changes compared to envelope.VM from the document store
                envelope.VM <- viewModel
                envelope.Version <- envelope.Version |> DocumentVersion.increment

                session.Store(envelope)
                do! session.SaveChangesAsync()
            }

        member this.Dispose() =
            if not (isNull session) then
                session.Dispose()

        member this.DisposeAsync() =
            if not (isNull session) then
                session.DisposeAsync()
            else
                ValueTask.CompletedTask

[<Sealed>]
type MartenDbProjectionStore<'TViewModel when 'TViewModel: null>(documentStore: IDocumentStore) =
    // in the case of MartenDb, there are no explicit documents collections (like e.g. in MongoDb)
    // so MartenDbProjectionStore simply forwards the documentStore instance
    // (which is supposed to be a singleton) to MartenDbDocumentCollection
    interface IProjectionStore<'TViewModel> with
        member this.OpenDocumentCollection<'TViewModel> _ =
            Task.FromResult(new MartenDbDocumentCollection<'TViewModel>(documentStore))

        // MartenDb IDocumentStore instance lifecycle is managed by the application host
        // hence no disposing is necessary here
        member this.Dispose() = ()
        member this.DisposeAsync() = ValueTask.CompletedTask

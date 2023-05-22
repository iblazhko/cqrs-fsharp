namespace CQRS.Ports.ProjectionStore

open System
open System.Threading.Tasks

(*
These interfaces are meant for interoperability with actual event store
implementations, so we are using interfaces and Tasks here, and do not
use advanced type system (e.g. discriminated unions etc.)
*)

type DocumentCollectionId = string
exception InvalidDocumentCollectionIdException

module DocumentCollectionId =
    let newId () =
        Guid.NewGuid().ToString("N").ToUpperInvariant()

    let create (s: string) =
        if String.IsNullOrWhiteSpace(s) then
            raise InvalidDocumentCollectionIdException

        DocumentCollectionId(s.ToUpperInvariant())

    let value (s: DocumentCollectionId) = s


type DocumentId = string
exception InvalidDocumentIdException

module DocumentId =
    let newId () =
        Guid.NewGuid().ToString("N").ToUpperInvariant()

    let create (s: string) =
        if String.IsNullOrWhiteSpace(s) then
            raise InvalidDocumentIdException

        DocumentId(s.ToUpperInvariant())

    let value (s: DocumentId) = s


type DocumentVersion = int64

module DocumentVersion =
    [<Literal>]
    let New: DocumentVersion = 0L

    let increment v : DocumentVersion = v + 1L

[<Interface>]
type IDocumentCollection<'TViewModel> =
    inherit IDisposable
    inherit IAsyncDisposable
    abstract member GetById: DocumentId -> Task<'TViewModel option>
    abstract member Update: DocumentId * 'TViewModel -> Task
    abstract member Update: DocumentId * ('TViewModel -> 'TViewModel) -> Task

[<Interface>]
type IDocumentStore<'TViewModel> =
    inherit IDisposable
    inherit IAsyncDisposable
    abstract member OpenDocumentCollection<'TViewModel> : DocumentCollectionId -> Task<IDocumentCollection<'TViewModel>>

exception ConcurrencyException of DocumentId * string

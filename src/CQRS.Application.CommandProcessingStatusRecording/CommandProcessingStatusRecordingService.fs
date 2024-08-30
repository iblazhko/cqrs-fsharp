namespace CQRS.Application.CommandProcessingStatusRecording

open System
open System.Threading.Tasks
open CQRS.Ports.Messaging
open CQRS.Ports.ProjectionStore

type Status =
    | Unknown
    | Processing
    | Completed
    | Rejected
    | Failed

type CommandProcessingStatus =
    { CommandId: MessagingId
      CorrelationId: MessagingId
      CausationId: MessagingId
      CommandType: string
      CommandBody: string
      Response: string
      Status: Status
      RequestedAt: DateTimeOffset
      UpdatedAt: DateTimeOffset option }

[<AllowNullLiteral>]
type CommandProcessingStatusViewModel() =
    member val CommandId: MessagingId = MessagingId.Empty with get, set
    member val CorrelationId: MessagingId = MessagingId.Empty with get, set
    member val CausationId: MessagingId = MessagingId.Empty with get, set
    member val CommandType: string = String.Empty with get, set
    member val CommandBody: string = String.Empty with get, set
    member val Response: string = String.Empty with get, set
    member val Status: string = String.Empty with get, set
    member val RequestedAt: string = String.Empty with get, set
    member val UpdatedAt: string = String.Empty with get, set
    override this.ToString() = System.Text.Json.JsonSerializer.Serialize(this)

module CommandProcessingStatus' =
    let toDTO (domain: CommandProcessingStatus) =
        let dto = CommandProcessingStatusViewModel()
        dto.CommandId <- domain.CommandId
        dto.CorrelationId <- domain.CorrelationId
        dto.CausationId <- domain.CausationId
        dto.CommandType <- domain.CommandType
        dto.CommandBody <- domain.CommandBody
        dto.Response <- domain.Response

        dto.Status <-
            match domain.Status with
            | Unknown -> "Unknown"
            | Processing -> "Processing"
            | Completed -> "Completed"
            | Rejected -> "Rejected"
            | Failed -> "Failed"

        dto.RequestedAt <- domain.RequestedAt.ToString("O")

        dto.UpdatedAt <-
            match domain.UpdatedAt with
            | Some d -> d.ToString("O")
            | None -> String.Empty

        dto

    let ofDTO (dto: CommandProcessingStatusViewModel) =
        { CommandId = dto.CommandId
          CorrelationId = dto.CorrelationId
          CausationId = dto.CausationId
          CommandType = dto.CommandType
          CommandBody = dto.CommandBody
          Response = dto.Response
          Status =
            match dto.Status with
            | "Processing" -> Processing
            | "Completed" -> Completed
            | "Rejected" -> Rejected
            | "Failed" -> Failed
            | _ -> Unknown
          RequestedAt =
            (let parsed, d = DateTimeOffset.TryParse(dto.RequestedAt)
             if parsed then d else DateTimeOffset.MinValue)
          UpdatedAt =
            (let parsed, d = DateTimeOffset.TryParse(dto.UpdatedAt)
             if parsed then Some d else None) }

type CommandProcessingRequest() =
    member val CommandId: MessagingId = MessagingId.Empty with get, set
    member val CorrelationId: MessagingId = MessagingId.Empty with get, set
    member val CausationId: MessagingId = MessagingId.Empty with get, set
    member val CommandType: string = String.Empty with get, set
    member val CommandBody: string = String.Empty with get, set
    member val RequestedAt: DateTimeOffset = DateTimeOffset.MinValue with get, set

[<Interface>]
type ICommandProcessingStatusRecordingService =
    abstract member RecordCommandProcessingStarted: CommandProcessingRequest -> Task
    abstract member RecordCommandProcessingCompleted: MessagingId * DateTimeOffset -> Task
    abstract member RecordCommandProcessingRejected: MessagingId * DateTimeOffset * string -> Task
    abstract member RecordCommandProcessingFailed: MessagingId * DateTimeOffset * string -> Task


module CommandProcessingStatusDocumentCollection =
    let documentCollectionId: DocumentCollectionId = "CommandProcessingStatus"
    let getDocumentId (commandId: MessagingId) = commandId.ToString()

[<Sealed>]
type CommandProcessingStatusRecordingService(projectionStore: IProjectionStore<CommandProcessingStatusViewModel>) =
    let getTimestamp (dt: DateTimeOffset) = $"{dt:O}"

    interface ICommandProcessingStatusRecordingService with
        member this.RecordCommandProcessingStarted(request: CommandProcessingRequest) =
            task {
                use! session = projectionStore.OpenDocumentCollection(CommandProcessingStatusDocumentCollection.documentCollectionId)
                let documentId = request.CommandId |> CommandProcessingStatusDocumentCollection.getDocumentId

                do!
                    session.Update(
                        documentId,
                        (fun vm ->
                            vm.CommandId <- request.CommandId
                            vm.CorrelationId <- request.CorrelationId
                            vm.CausationId <- request.CausationId
                            vm.CommandType <- request.CommandType
                            vm.CommandBody <- request.CommandBody
                            vm.Response <- String.Empty
                            vm.Status <- "Requested"
                            vm.RequestedAt <- request.RequestedAt |> getTimestamp
                            vm.UpdatedAt <- String.Empty
                            vm)
                    )
            }

        member this.RecordCommandProcessingCompleted(commandId: MessagingId, processedAt: DateTimeOffset) =
            task {
                use! session = projectionStore.OpenDocumentCollection(CommandProcessingStatusDocumentCollection.documentCollectionId)
                let documentId = commandId |> CommandProcessingStatusDocumentCollection.getDocumentId

                do!
                    session.Update(
                        documentId,
                        (fun vm ->
                            vm.Response <- String.Empty
                            vm.Status <- "Completed"
                            vm.UpdatedAt <- processedAt |> getTimestamp
                            vm)
                    )
            }

        member this.RecordCommandProcessingRejected
            (
                commandId: MessagingId,
                processedAt: DateTimeOffset,
                rejectionReason: string
            ) =
            task {
                use! session = projectionStore.OpenDocumentCollection(CommandProcessingStatusDocumentCollection.documentCollectionId)
                let documentId = commandId |> CommandProcessingStatusDocumentCollection.getDocumentId

                do!
                    session.Update(
                        documentId,
                        (fun vm ->
                            vm.Response <- rejectionReason
                            vm.Status <- "Rejected"
                            vm.UpdatedAt <- processedAt |> getTimestamp
                            vm)
                    )
            }

        member this.RecordCommandProcessingFailed
            (
                commandId: MessagingId,
                processedAt: DateTimeOffset,
                failure: string
            ) =
            task {
                use! session = projectionStore.OpenDocumentCollection(CommandProcessingStatusDocumentCollection.documentCollectionId)
                let documentId = commandId |> CommandProcessingStatusDocumentCollection.getDocumentId

                do!
                    session.Update(
                        documentId,
                        (fun vm ->
                            vm.Response <- failure
                            vm.Status <- "Failed"
                            vm.UpdatedAt <- processedAt |> getTimestamp
                            vm)
                    )
            }

module GetCommandProcessingStatusQueryRepository =
    let getDocument (projectionStore: IProjectionStore<CommandProcessingStatusViewModel>) (commandId: MessagingId) =
        task {
            use! collection = projectionStore.OpenDocumentCollection(CommandProcessingStatusDocumentCollection.documentCollectionId)
            let documentId = commandId |> CommandProcessingStatusDocumentCollection.getDocumentId
            let! document = collection.GetById(documentId)
            return document
        }

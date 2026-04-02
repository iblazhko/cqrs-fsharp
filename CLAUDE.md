# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build and Test Commands

The build system uses PowerShell (`build.ps1`). Prerequisites: .NET 10.0 SDK, PowerShell 7+, Docker with Docker Compose.

```bash
./build.ps1                            # Full build + all tests (equivalent to -Target FullBuild)
./build.ps1 Dotnet.Test                # Run tests only
./build.ps1 Prune                      # Remove intermediate build files
./build.ps1 DockerCompose.Start        # Start full system with infrastructure (foreground)
./build.ps1 DockerCompose.StartDetached  # Start in detached mode
./build.ps1 DockerCompose.Stop         # Stop detached system
./build.ps1 Prune.Docker               # Clean Docker containers/images (stop first)
```

To run a single test project directly:

```bash
dotnet test src/CQRS.Domain.Tests/CQRS.Domain.Tests.fsproj
dotnet test src/CQRS.API.Tests/CQRS.API.Tests.fsproj
```

The solution file is at `src/CQRS.slnx` (SLNX format, .NET 10).

## Architecture

This is an F# CQRS + Event Sourcing + DDD reference implementation using **Functional Core, Imperative Shell** + **Ports and Adapters (Hexagonal Architecture)**.

### Layers and Dependency Rules

**Core** (pure functional, no I/O dependencies):

- `CQRS.Domain` — aggregates as pure functions: `(State, Command) → Result<Event list, Error>`
- `CQRS.DTO/V1` — serialization-friendly POCOs (primitive CLR types only) for commands/events
- `CQRS.Mapping` — bidirectional DTO ↔ Domain model mappers
- `CQRS.EntityIds` — entity ID types
- Core MUST NOT depend on Ports, Application, or Projections

**Ports** (abstractions, no concrete deps):

- `CQRS.Ports.EventStore` — `IEventStore`, `IEventStreamSession`
- `CQRS.Ports.MessageBus` — pub/sub abstraction
- `CQRS.Ports.ProjectionStore` — read model persistence abstraction
- Ports MUST NOT depend on Core or Application

**Adapters** (one adapter per port):

- `CQRS.Adapters.InMemoryEventStore/ProjectionStore/MessageBus` — dev/test implementations
- `CQRS.Adapters.MartenDbEventStore/ProjectionStore` — PostgreSQL-backed via MartenDB
- `CQRS.Adapters.WolverineMessageBus` — RabbitMQ-backed via Wolverine
- Adapters MUST NOT depend on Core or Application; each adapter implements exactly ONE port

**Application** (functional with effects via Ports):

- `CQRS.Application` — generic command/event DTO handlers; uses EventStore and ProjectionStore ports
- `CQRS.Application.WolverineConsumers` — unwraps Wolverine context, passes DTO to Application handler; NO business logic here
- `CQRS.Application.CommandProcessingStatusRecording` — async command status tracking
- MUST NOT depend on Adapters; MUST NOT depend on API

**Projections** (functional with effects via Ports):

- `CQRS.Projections` — event → view model projection logic
- `CQRS.Projections.ViewModels` — denormalized read models
- `CQRS.Projections.Repositories` — view model retrieval
- `CQRS.Projections.WolverineConsumers` — same pattern as Application consumers
- MUST NOT depend on Adapters or Application

**API** (imperative shell, ASP.NET Core Minimal API):

- `CQRS.API` — command and query endpoints; uses MessageBus port (commands) and ProjectionStore port (queries)
- `CQRS.API.Host` — executable host on port 17322
- MUST NOT depend on Adapters or Application

**Application Host** (imperative shell):

- `CQRS.Application.Host` — executable host on port 17321; processes commands and runs projections
- `CQRS.Infrastructure` — shared DI registration, health checks, startup logic
- `CQRS.Configuration` — settings from `appsettings.json`/environment variables
- `API.Host` MUST NOT depend on Application; `Application.Host` MUST NOT depend on API

**Client**:

- `CQRS.CLI` — command-line HTTP client; MUST ONLY depend on `CQRS.DTO` and `CQRS.Projections.ViewModels`

**These dependency rules are enforced by `CQRS.Architecture.Tests/DependenciesTests.fs`.**

### Domain Model

The domain models the `Inventory` aggregate with commands (`CreateInventory`, `RenameInventory`, `AddItemsToInventory`, `RemoveItemsFromInventory`, `DeactivateInventory`) and corresponding events. Aggregates are pure functions — they do not store or publish events; the Application layer handles that.

### Infrastructure (Docker Compose)

- **PostgreSQL 18.3** — event store (MartenDB) + projection store
- **RabbitMQ 4.2** — message broker (Wolverine)
- Two service containers: `server-application` (port 17321) and `server-api` (port 17322)

### Key Libraries

- **FsToolkit.ErrorHandling** — `Result`/`Async` computation expressions throughout
- **FPrimitive** — value object constraints in Core
- **MartenDB** — PostgreSQL event store and document projections
- **Wolverine** — message bus with RabbitMQ transport
- **xUnit + FsUnit** — test framework
- **Serilog** — structured logging

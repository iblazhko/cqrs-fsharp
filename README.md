# CQRS Made Functional

Example of a simple inventory management system implemented in F# using DDD,
CQRS, and Event Sourcing.

This project is intended to implement a solution with modularity
and versioning (eventually), and serve as a boilerplate template for
real-life projects.

## Solution Overview

Solution uses .NET7 / F# and has following major parts:

- *Core domain* components. Only types and pure functions are allowed here.
  - `CQRS.Domain`: Implements domain model and domain services.
  - `CQRS.DTO`: Commands and Events DTOs that are shared across domain and
    application services projects, and stored in event streams. DTOs define
    shape of messages that go over the wire, they designed to be
    implementation-neutral and serialization-friendly. Essentially they are
    [POCO](https://en.wikipedia.org/wiki/Plain_old_CLR_object)s that only use
    primitive CLR types.
  - `CQRS.Mapping`: Implements mapping between DTOs and domain model.
- *Application* services that are aware of the outside world and intended
    to have effects - consume and publish messages, read and write DB data,
    make external HTTP/GRPC calls etc.
  - `CQRS.Application`: implements application services; handles Commands sent
    from the API.
  - `CQRS.Application.Projections`: projects domain Events to view models
    persisted in a document-based ProjectionStore.
- *API* components
  - `CQRS.API`: sends Commands to Application (command API),
    uses ProjectionStore to retrieve view models (query API).
- *Ports*
  - `CQRS.Ports.EventStore`: EventStore abstraction for event sourcing /
    event store
  - `CQRS.Ports.MessageBus`: message bus abstraction for sending commands and
    publishing events. Note that registration of messages consumers is not
    a part of this abstraction - it is a part of Application host that rely
    on a specific MessageBus adapter.
  - `CQRS.Ports.ProjectionStore`: abstraction for persisting event
    projections in denormalized form (document store). Normalized form
    (relational database) projections are not a part of this abstraction
    at the moment, but could be added if needed.
- *Adapters*
  - `CQRS.Adapters.InMemoryEventStore`: in-memory adapter for *EventStore* port
  - `CQRS.Adapters.InMemoryMessageBus`: in-memory adapter for *MessageBus* port
  - `CQRS.Adapters.InMemoryProjectionStore`: in-memory adapter for
    *ProjectionStore* port
  - `CQRS.Adapters.MartenDbEventStore`: [MartenDB](https://martendb.io/) adapter
    for *EventStore* port
  - `CQRS.Adapters.MartenDbProjectionStore`: [MartenDB](https://martendb.io/)
    adapter for *ProjectionStore* port
  - `CQRS.Adapters.MassTransitMessageBus`:
    [MassTransit](https://masstransit.io/) /
    [RabbitMQ](https://www.rabbitmq.com/) adapter for *MessageBus* port
- Client (`CQRS.Client`): example of an external client that interacts with
  the system via API.
- Benchmark tests (`benchmark`). (WIP)

For educational purposes *API* and *Application* have separate hosts
(`CQRS.API.Host` and `CQRS.Application.Host` respectively). In a real life
projects these two hosts can be combined into one, or *Application* host
can be split into smaller parts for better scalability (e.g. to separate
commands handling and maintaining projections).

## Building and Running

### Prerequisites

- .NET SDK 7.0: <https://dotnet.microsoft.com/download>
- PowerShell Core: <https://github.com/powershell/powershell>
- Docker with Docker Compose: <https://www.docker.com/>

#### Development Environment

Development environment is a personal choice, here are some options:

- [JetBrains Rider](https://www.jetbrains.com/rider/)
- [Visual Studio](https://visualstudio.microsoft.com/)
- [Visual Studio Code](https://code.visualstudio.com/) with following
  extensions:
  - Ionide F# (`ionide.ionide-fsharp`)
  - PowerShell (`ms-vscode.powershell`)
  - Docker (`ms-azuretools.vscode-docker`)

### Build + Test

To build the solution and run tests:

```bash
./build.ps1
```

> NOTE. Command above is equivalent to
>
> ```bash
> ./build.ps1 -Target FullBuild
> ```

#### Cleanup

To completely remove all the intermediate build files:

```bash
./build.ps1 Prune
```

To cleanup Docker containers and images, first make sure that the system
is stopped (see `DockerCompose.Stop` target below).

Then run command

```bash
./build.ps1 Prune.Docker
```

### Run

To run the solution:

```bash
./build.ps1 DockerCompose.Start
```

This will start Docker Compose project and will display combined console logs
from all the components involved. The system will stop when any of the
containers terminates. Use `Ctrl+C` to terminate manually.

```bash
./build.ps1 DockerCompose.StartDetached
```

This will run the Docker Compose project in detached mode;
use `docker logs` to inspect individual parts.

To stop a system running in the detached mode:

```bash
./build.ps1 DockerCompose.Stop
```

## References

1. Fowler, Martin. “Bliki: CQRS” martinfowler.com, 2011-07-14, <https://martinfowler.com/bliki/CQRS.html>.
1. Wlaschin, Scott. “Domain Modeling Made Functional”. P1.0, The Pragmatic Programmers, 2018-01, <https://pragprog.com/titles/swdddf/domain-modeling-made-functional/>. 
1. Young, Gregory. “CQRS Documents” wordpress.com, 2010-11, <https://cqrs.files.wordpress.com/2010/11/cqrs_documents.pdf>.
1. Young, Gregory. “Simple CQRS” github.com, 2015-01-13, <https://github.com/gregoryyoung/m-r>.

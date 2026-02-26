# CQRS Made (mostly) Functional

[![CI Workflow](https://github.com/iblazhko/cqrs-fsharp/actions/workflows/ci-workflow.yml/badge.svg?branch=main)](https://github.com/iblazhko/cqrs-fsharp/actions/workflows/ci-workflow.yml)

Example of a simple inventory management system implemented in F# using DDD,
CQRS, and Event Sourcing.

This project is intended to implement a solution with modularity
and versioning (eventually), and serve as a boilerplate template for
real-life projects.

> It is still very much work in progress, so definitely not suitable to be
> a template for real-life projects just yet.

## Solution Overview

Solution uses .NET 10 / F# and has following major parts:

- *Core domain* components. Only types and pure functions are allowed here.
- *Application* services that are aware of the outside world and intended
  to have effects - consume and publish messages, read and write DB data,
  make external HTTP/GRPC calls etc.
- *API* sends Commands to *Application* (command API), and uses
  *ProjectionStore* to retrieve view models (query API).
- *Ports*. Abstractions for event store, projections store, and message bus.
- *Adapters*. Implementations of *Ports* abstraction that (typically) use
  external services, e.g. MartenDB or RabbitMQ.
- *CLI*: example of an external client that interacts with
  the system via *API*.

See [“Architecture”](./doc/architecture.md) and
[“Solution Structure”](./doc/solution-structure.md) documents for more
information.

## Building and Running

### Prerequisites

- .NET 10.0 SDK: <https://dotnet.microsoft.com/download>
- PowerShell: <https://github.com/powershell/powershell>
- Docker with Docker Compose: <https://www.docker.com/>,
  or Podman <https://podman.io/> with [`docker` wrapper](https://podman-desktop.io/docs/migrating-from-docker/emulating-docker-cli-with-podman)

#### Development Environment

Development environment is a personal choice, here are some options:

- [JetBrains Rider](https://www.jetbrains.com/rider/)
- [Visual Studio](https://visualstudio.microsoft.com/)
- [Visual Studio Code](https://code.visualstudio.com/) with following
  extensions:
    - Ionide F# (`ionide.ionide-fsharp`)
    - PowerShell (`ms-vscode.powershell`)
    - Docker (`ms-azuretools.vscode-docker`)

PowerShell helper script `build.ps1` can be used to build and run the solution,
see [“Build and Run”](./doc/build-and-run.md) document for more information.

### Interacting with the running system

See [“Interacting with the system using API”](./doc/cli-api.md) document.

## References

1. Wlaschin, Scott. “Domain Modeling Made Functional”, The Pragmatic Programmers,
   2018-01, <https://pragprog.com/titles/swdddf/domain-modeling-made-functional/>.
2. Evans, Eric. “Domain-Driven Design: Tackling Complexity in the Heart of Software”, Addison-Wesley Professional, 2003-09-11
3. Fowler, Martin. “Bliki: CQRS” martinfowler.com, 2011-07-14, <https://martinfowler.com/bliki/CQRS.html>.
4. Young, Gregory. “CQRS Documents” wordpress.com,
   2010-11, <https://cqrs.files.wordpress.com/2010/11/cqrs_documents.pdf>.
5. Young, Gregory. “Simple CQRS” github.com, 2015-01-13, <https://github.com/gregoryyoung/m-r>.
6. Fowler, Martin. “Event Sourcing” martinfowler.com, 2005-12-12, <https://martinfowler.com/eaaDev/EventSourcing.html>.
7. Microsoft Learning. “Event Sourcing pattern”, learn.microsoft.com <https://learn.microsoft.com/en-us/azure/architecture/patterns/event-sourcing>.

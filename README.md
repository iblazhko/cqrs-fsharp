# CQRS Made Functional

Example of a simple inventory management system implemented in F# using DDD,
CQRS, and Event Sourcing.

This project is intended to implement a solution with modularity
and versioning (eventually), and serve as a boilerplate template for
real-life projects.

## Solution Overview

Solution uses .NET7 / F# and has following major parts:

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

See [“Solution Structure”](./doc/solution-structure.md) document for more information.

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

PowerShell helper script `build.ps1` can be used to build and run the solution,
see [“Build and Run”](./doc/build-and-run.md) document for more information.

### Interacting with the running system

See [“Interacting with the system using API”](./doc/cli-api.md) document.

## References

1. Fowler, Martin. “Bliki: CQRS” martinfowler.com, 2011-07-14, <https://martinfowler.com/bliki/CQRS.html>.
1. Wlaschin, Scott. “Domain Modeling Made Functional”. P1.0, The Pragmatic Programmers, 2018-01, <https://pragprog.com/titles/swdddf/domain-modeling-made-functional/>.
1. Young, Gregory. “CQRS Documents” wordpress.com, 2010-11, <https://cqrs.files.wordpress.com/2010/11/cqrs_documents.pdf>.
1. Young, Gregory. “Simple CQRS” github.com, 2015-01-13, <https://github.com/gregoryyoung/m-r>.

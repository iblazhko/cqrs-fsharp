# Functional Core, Imperative Shell

This solution uses a mix of *Functional Core, Imperative Shell* pattern and
*Ports and Adapters* pattern (a.k.a. *Hexagonal Architecture*).

## Functional Core

Pure functional core consists of domain model, [DTO](./dto.md)s, and mappers
that translate between domain model and DTOs.

## Imperative Shell

Non-pure imperative shell consists of two main parts:

* Host executable projects. These depend on Microsoft Hosting and Microsoft
  Dependency Injection components, and while implemented in F#, code there
  can be considered rather imperative. Here we prepare environment - read
  configuration from `appsettings.json` / environment variables, register
  adapters in MS DI, register MassTransit consumers etc. Finally we start
  the host by using methods provided in Microsoft Hosting.

* Input/Output. Here we can use Ports for communicating with external services
  (mainly EventStore and Projections Store), and code can mix imperative
  and functional styles.
  * API calls are handled by ASP.NET Core, with a bit of help from
    `FSharp.MinimalApi` library to simplify endpoints registration.
    API can send commands using Message Bus Port, or query view models
    using ProjectionStore Port
  * Messages are handled using MassTransit, and we define MassTransit-specific
    message consumers that unwrap MassTransit consume context and pass on DTO
    into a generic Application of Projection DTO handler. Command DTO handler
    uses EventStore Port (and others), Event DTO handler uses ProjectionStore
    Port.

## References

1. <https://kennethlange.com/functional-core-imperative-shell/>
2. <https://blog.ploeh.dk/2020/03/02/impureim-sandwich/>
3. <https://en.wikipedia.org/wiki/Hexagonal_architecture_(software)>

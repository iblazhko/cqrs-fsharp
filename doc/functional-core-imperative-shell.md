# Functional Core, Imperative Shell

This solution uses a mix of *Functional Core, Imperative Shell* pattern and
*Ports and Adapters* pattern (a.k.a. *Hexagonal Architecture*).

## Functional Core

Pure functional core consists of domain model, [DTO](./dto.md)s, and mappers
that translate between domain model and DTOs.

Domain *Aggregates* (in DDD-speak) are pure functions that take current state
and domain command -- both represented as domain models -- and return sequence
of domain events describing changes in the system. Domain *Aggregates* are *not*
responsible for storing or publishing these events, this is the responsibility
of the Application, as described below.

## Application Services

Application services use functional style, but allowed to have effects --
publishing events using *MessageBus Port*, storing events using
*EventStore Port*, and communicating with external services.

> *WIP*. In principle, this part can be changed to be pure functions as well,
> with all of the I/O invocations lifted to the outer shell described below.

Application implements generic Command DTO handler that is using
*EventStore Port*, and Event DTO handler that uses *ProjectionStore Port*.

## Imperative Shell

Non-pure imperative shell consists of two main parts:

* Host executable projects. These depend on Microsoft Hosting and Microsoft
  Dependency Injection components, and while implemented in F#, code there
  can be considered rather imperative. Here we prepare environment - read
  configuration from `appsettings.json` / environment variables, register
  adapters in MS DI, register MassTransit consumers etc. Finally we start
  the host by using methods provided in Microsoft Hosting.

* Input triggers. Here we take inputs from either API calls or incoming
  messages, and call Application layer (as described above) that
  follows functional style, but directly uses Ports for communicating with
  external services (mainly *EventStore Port* and *Projections Store Port*).
  Code in the imperative shell itself can mix imperative and functional styles.
  * API calls are handled by ASP.NET Core Minimal API.
    API can send commands using *MessageBus Port*, or query view models
    using *ProjectionStore Port*
  * Messages are handled using MassTransit, and we define MassTransit-specific
    message consumers that unwrap MassTransit consume context and pass on DTO
    into a generic Application of Projection DTO handler.

## References

1. <https://kennethlange.com/functional-core-imperative-shell/>
2. <https://blog.ploeh.dk/2020/03/02/impureim-sandwich/>
3. <https://en.wikipedia.org/wiki/Hexagonal_architecture_(software)>

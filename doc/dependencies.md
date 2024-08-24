# Solution Dependencies Rules

1. **`Core`**. Projects in this group (`Domain`, `DTO`, `Mapping`) MUST NOT
   depend on any project from `Server` (i.e. there should be no dependencies on
   Ports, Application, or Projections). The only external libraries used there
   are `FPrimitive` (for declaring value objects constraints),
   `FSToolkit.ErrorHandling` (for generic `ErrorsByTag` type and errors
   handling), and `Nanoid` (for short ids generation).
2. **`Server`**
    1. `Application`
        1. MUST NOT depend on any project from `Adapters`, only on `Ports`
        2. MUST NOT depend on `API`
        3. MAY depend on `Projections.Repositories` (retrieving of view models),
           but this is not needed a.t.m.
        4. Depends on `Domain` to access domain Commands and Events definitions,
           DTO ⬄ Domain mappers, and Domain Aggregates.
    2. `Projections`
        1. MUST NOT depend on any project from `Adapters`, only on `Ports`
        2. MUST NOT depend on `Application`
        3. MUST NOT depend on `API`
        4. Depends on `Domain` to access domain Events definitions and
           DTO ⬄ Domain mappers
    3. `API`
        1. MUST NOT depend on any project from `Adapters`, only on `Ports`
        2. Depends on `CQRS.DTO` (specifically on command DTOs) and uses
           DTOs as request body in command API.
        3. Depends on `CQRS.Projections.ViewModels` and exposes them as
           Query API response
    4. `Ports`
        1. MUST NOT depend on any project from `Core`
        2. MUST NOT depend on any project from `Application` or `Projections`
        3. One `Port` SHOULD NOT depend on another `Port`. Ideally a `Port` should
           be fully self-contained. In theory, there can be exceptions and
           if a port encapsulates cross-cutting concern it could be used in
           other ports, but this should be used with caution
    5. `Adapters`
        1. MUST NOT depend on any project from `Core`
        2. MUST NOT depend on any project from `Application` or `Projections`
        3. An `Adapter` MUST ONLY implement one corresponding `Port`
    6. Code in `*.MassTransitConsumers` projects MUST ONLY unwrap MassTransit
       consume context and pass DTO to `Application` or `Projections`
       DTO handler, there should be no application logic there as such
    7. `Hosts`
        1. `Application.Host` MUST NOT depend on `API`
        2. `API.Host` MUST NOT depend on `Application`
3. **`Client`** MUST ONLY depend on `CQRS.DTO` (command DTOs) and
   `CQRS.Projections.ViewModels`

These rules are checked in [Architecture Tests](../src/CQRS.Architecture.Tests/DependenciesTests.fs).

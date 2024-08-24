module DependenciesTests

open System.Reflection
open Xunit

let private domainLayer =
    [ Assembly.Load("CQRS.Domain")
      Assembly.Load("CQRS.EntityIds")
      Assembly.Load("CQRS.Mapping") ]

let private domainDtoLayer = [ Assembly.Load("CQRS.DTO") ]

let private applicationLayer =
    [ Assembly.Load("CQRS.Application")
      Assembly.Load("CQRS.Application.MassTransitConsumers") ]

let private projectionsLayer =
    [ Assembly.Load("CQRS.Projections")
      Assembly.Load("CQRS.Projections.MassTransitConsumers") ]

let private projectionsRepositoriesLayer =
    [ Assembly.Load("CQRS.Projections.Repositories") ]

let private projectionsViewModelsLayer =
    [ Assembly.Load("CQRS.Projections.ViewModels") ]

let private apiLayer = [ Assembly.Load("CQRS.API") ]

let private portsLayer =
    [ Assembly.Load("CQRS.Ports.EventStore")
      Assembly.Load("CQRS.Ports.MessageBus")
      Assembly.Load("CQRS.Ports.ProjectionStore") ]

let private adaptersLayer =
    [ Assembly.Load("CQRS.Adapters.InMemoryEventStore")
      Assembly.Load("CQRS.Adapters.InMemoryMessageBus")
      Assembly.Load("CQRS.Adapters.InMemoryProjectionStore")
      Assembly.Load("CQRS.Adapters.MartenDbEventStore")
      Assembly.Load("CQRS.Adapters.MartenDbProjectionStore")
      Assembly.Load("CQRS.Adapters.MassTransitMessageBus") ]

let private apiHostLayer = [ Assembly.Load("CQRS.API.Host") ]
let private applicationHostLayer = [ Assembly.Load("CQRS.Application.Host") ]
let private cliHostLayer = [ Assembly.Load("CQRS.CLI") ]

let private coreGroup = domainLayer @ domainDtoLayer
let private serverGroup = applicationLayer @ projectionsLayer @ projectionsViewModelsLayer @ portsLayer @ adaptersLayer

let private assemblyDependencies (assemblyUnderTest: Assembly) (otherAssembly: Assembly) =
    Seq.ofArray (assemblyUnderTest.GetReferencedAssemblies())
    |> Seq.choose (fun x -> if x.FullName = otherAssembly.FullName then Some x else None)

let private assemblyLayerDependencies (assemblyUnderTest: Assembly) (otherLayer: Assembly list) =
    Seq.ofList otherLayer
    |> Seq.fold (fun d a -> Seq.append d (assemblyDependencies assemblyUnderTest a)) Seq.empty

let private assemblyHasDependency (assemblyUnderTest: Assembly) (otherAssembly: Assembly) =
    Seq.ofArray (assemblyUnderTest.GetReferencedAssemblies())
    |> Seq.exists (fun x -> x.FullName = otherAssembly.FullName)

let private layerHasDependency (layerUnderTest: Assembly list) (otherLayer: Assembly list) =
    let assembliesUnderTest = Seq.ofList layerUnderTest
    let otherAssemblies = Seq.ofList otherLayer

    assembliesUnderTest
    |> Seq.exists (fun x -> otherAssemblies |> Seq.exists (assemblyHasDependency x))

[<Fact>]
let ``Core MUST NOT depend on Server`` () =
    Assert.False(layerHasDependency coreGroup serverGroup)

[<Fact>]
let ``Application MUST NOT depend on Adapters, only on Ports`` () =
    Assert.True(layerHasDependency applicationLayer portsLayer)
    Assert.False(layerHasDependency applicationLayer adaptersLayer)

[<Fact>]
let ``Application MUST NOT depend on Projections`` () =
    Assert.False(layerHasDependency applicationLayer projectionsLayer)
    Assert.False(layerHasDependency applicationLayer projectionsViewModelsLayer)

[<Fact>]
let ``Application MUST NOT depend on API`` () =
    Assert.False(layerHasDependency applicationLayer apiLayer)

[<Fact>]
let ``Application depends on Domain`` () =
    Assert.True(layerHasDependency applicationLayer domainLayer)
    Assert.True(layerHasDependency applicationLayer domainDtoLayer)

[<Fact>]
let ``Projections MUST NOT depend on Adapters, only on Ports`` () =
    Assert.True(layerHasDependency projectionsLayer portsLayer)
    Assert.False(layerHasDependency projectionsLayer adaptersLayer)

[<Fact>]
let ``Projections MUST NOT depend on Application`` () =
    Assert.False(layerHasDependency projectionsLayer applicationLayer)
    Assert.False(layerHasDependency projectionsViewModelsLayer applicationLayer)

[<Fact>]
let ``Projections MUST NOT depend on API`` () =
    Assert.False(layerHasDependency projectionsLayer apiLayer)

[<Fact>]
let ``Projections depend on Domain`` () =
    Assert.True(layerHasDependency projectionsLayer domainLayer)
    Assert.True(layerHasDependency projectionsLayer domainDtoLayer)

[<Fact>]
let ``API MUST NOT depend on Adapters, only on Ports`` () =
    Assert.True(layerHasDependency apiLayer portsLayer)
    Assert.False(layerHasDependency apiLayer adaptersLayer)

[<Fact>]
let ``API MUST NOT depend on Application`` () =
    Assert.False(layerHasDependency apiLayer applicationLayer)

[<Fact>]
let ``API MUST NOT depend on Projections internal implementation`` () =
    Assert.False(layerHasDependency apiLayer projectionsLayer)

[<Fact>]
let ``API depends on Projections Repositories`` () =
    Assert.True(layerHasDependency apiLayer projectionsRepositoriesLayer)

[<Fact>]
let ``API depends on Projections ViewModels`` () =
    Assert.True(layerHasDependency apiLayer projectionsViewModelsLayer)

[<Fact>]
let ``Ports MUST NOT depend on Domain`` () =
    Assert.False(layerHasDependency portsLayer coreGroup)

[<Fact>]
let ``Ports MUST NOT depend on Application`` () =
    Assert.False(layerHasDependency portsLayer applicationLayer)

[<Fact>]
let ``Ports MUST NOT depend on Projections`` () =
    Assert.False(layerHasDependency portsLayer projectionsLayer)

[<Fact>]
let ``Port SHOULD NOT depend on another Port`` () =
    let ports = Seq.ofList portsLayer
    let portsDependingOnAnotherPort =
        ports
        |> Seq.choose
               (fun x -> if layerHasDependency
                              [x]
                              (ports |> Seq.choose (fun y -> if x.FullName = y.FullName then None else Some y) |> Seq.toList)
                         then Some (x.GetName().Name)
                         else None)
    Assert.Empty(portsDependingOnAnotherPort)

[<Fact>]
let ``Adapter MUST implement only one Port`` () =
    let adaptersImplementingMoreThanOnePort =
        adaptersLayer
        |> Seq.ofList
        |> Seq.choose(fun x -> if (assemblyLayerDependencies x portsLayer |> Seq.length) > 1 then Some (x.GetName().Name) else None)
    Assert.Empty(adaptersImplementingMoreThanOnePort)

[<Fact>]
let ``Adapters MUST NOT depend on Domain`` () =
    Assert.False(layerHasDependency adaptersLayer coreGroup)

[<Fact>]
let ``Adapters MUST NOT depend on Application`` () =
    Assert.False(layerHasDependency adaptersLayer applicationLayer)

[<Fact>]
let ``Adapters MUST NOT depend on Projections`` () =
    Assert.False(layerHasDependency adaptersLayer projectionsLayer)

[<Fact>]
let ``Application Host MUST NOT depend on API`` () =
    Assert.False(layerHasDependency applicationHostLayer apiLayer)

[<Fact>]
let ``API Host MUST NOT depend on Application`` () =
    Assert.False(layerHasDependency apiHostLayer applicationLayer)

[<Fact>]
let ``CLI Host MUST NOT depend on Application`` () =
    Assert.False(layerHasDependency cliHostLayer applicationLayer)

[<Fact>]
let ``CLI Host MUST NOT depend on Projections internal implementation`` () =
    Assert.False(layerHasDependency cliHostLayer projectionsLayer)

[<Fact>]
let ``CLI Host MUST NOT depend on Ports`` () =
    Assert.False(layerHasDependency cliHostLayer portsLayer)

[<Fact>]
let ``CLI Host MUST NOT depend on Adapters`` () =
    Assert.False(layerHasDependency cliHostLayer adaptersLayer)

[<Fact>]
let ``CLI Host MUST NOT depend on API internal implementation`` () =
    Assert.False(layerHasDependency cliHostLayer apiLayer)

[<Fact>]
let ``CLI Host depends on DTO`` () =
    Assert.True(layerHasDependency cliHostLayer domainDtoLayer)

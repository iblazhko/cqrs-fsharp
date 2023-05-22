namespace CQRS.Infrastructure

open System.Reflection

type EndpointsRegistration =
    { assemblies: Assembly list
      queuePrefix: string }

type HostEndpointsRegistration =
    { SendConventions: EndpointsRegistration option
      Consumers: EndpointsRegistration option }

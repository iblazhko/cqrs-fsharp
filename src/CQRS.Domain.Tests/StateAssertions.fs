module CQRS.Domain.Tests.StateAssertions

open Xunit

let assertState expected actual =
    Assert.Equal($"%A{expected}", $"%A{actual}")

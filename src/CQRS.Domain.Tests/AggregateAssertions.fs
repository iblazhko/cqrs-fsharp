module CQRS.Domain.Tests.AggregateAssertions

open Xunit

let assertAggregateSuccess expectedEvents result =
    match result with
    | Ok actualEvents -> Assert.Equal($"%A{expectedEvents}", $"%A{actualEvents}")
    | Error error -> Assert.Fail($"%A{error}")

let assertAggregateFailure expectedFailure result =
    match result with
    | Ok actualEvents -> Assert.Fail($"%A{actualEvents}")
    | Error error -> Assert.Equal($"%A{expectedFailure}", $"%A{error}")

module Elasticsearch.FSharp.Tests.Queries.TermQueryTests

open NUnit.Framework
open FsCheck.NUnit
open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization
open Elasticsearch.FSharp.Utility

[<Property>]
let ``"term" serializes correctly``(fieldName, fieldValue) = 
    let query =
        Search [
            Query (
                Term (fieldName, [ExactValue fieldValue])
            )
        ]
    let expected = sprintf """{"query":{"term":{"%s":{"value":"%s"}}}}"""
                       (Json.escapeString fieldName) (Json.escapeString fieldValue)
    let actual = toJson query
    Assert.That(actual, Is.EqualTo(expected))

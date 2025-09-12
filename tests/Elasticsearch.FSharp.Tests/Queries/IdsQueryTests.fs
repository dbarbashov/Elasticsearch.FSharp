module Elasticsearch.FSharp.Tests.Queries.IdsQueryTests

open NUnit.Framework
open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization

[<Test>]
let ``"ids" serializes correctly``() =
    let query =
        Search [
            Query (Ids ["foo"; "bar"])
        ]
    let expected = """{"query":{"ids":{"values":["foo","bar"]}}}"""
    let actual = toJson query
    Assert.That(actual, Is.EqualTo(expected))

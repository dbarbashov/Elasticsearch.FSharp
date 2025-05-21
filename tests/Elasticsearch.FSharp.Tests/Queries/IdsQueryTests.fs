module Elasticsearch.FSharp.Tests.Queries.IdsQueryTests

open NUnit.Framework
open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization
open Elasticsearch.FSharp.Utility
open Elasticsearch.FSharp.Tests.Helpers

[<Test>]
let ``"ids" serializes correctly``() =
    let query =
        Search [
            Query (Ids ["foo"; "bar"])
        ]
    let expected = """{"query":{"ids":{"values":["foo","bar"]}}}"""
    let actual = toJson query
    Assert.AreEqual(expected, actual)

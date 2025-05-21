module Elasticsearch.FSharp.Tests.Queries.MatchAllQueryTests

open NUnit.Framework
open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization
open Elasticsearch.FSharp.Utility

[<Test>]
let ``"match_all" serializes correctly``() =
    let query =
        Search [
            Query MatchAll
        ]
    let expected = """{"query":{"match_all":{}}}"""
    let actual = toJson query
    Assert.AreEqual(expected, actual)

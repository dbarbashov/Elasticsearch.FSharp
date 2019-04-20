module Elasticsearch.FSharp.Tests.Sort

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization
open NUnit.Framework

[<Test>]
let ``Sort serializes correctly``() =
    let query =
        Search [
            Sort ["myField", [Order SortOrder.Asc; Mode SortMode.Avg]]
            Query MatchAll
        ]
    let expected = """{"sort":[{"myField":{"order":"asc","mode":"avg"}}],"query":{"match_all":{}}}"""
    let actual = ToJson query
    Assert.AreEqual(expected, actual)
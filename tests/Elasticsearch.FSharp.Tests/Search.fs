module Elasticsearch.FSharp.Tests.Search

open NUnit.Framework
open FsCheck.NUnit

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization

[<Test>]
let ``Query serializes correctly``() =
    let query =
        Search [
            Query MatchAll
        ]
    let expected = """{"query":{"match_all":{}}}"""
    let actual = ToJson query
    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``Sort serializes correctly``() =
    let query =
        Search [
            Sort [
                "myField", [Order SortOrder.Asc; Mode SortMode.Avg]
            ]
        ]
    let expected = """{"sort":[{"myField":{"order":"asc","mode":"avg"}}]}"""
    let actual = ToJson query
    Assert.AreEqual(expected, actual)
    
[<Property>]
let ``From serializes correctly``(from) =
    let query =
        Search [
            From from
        ]
    let expected = sprintf """{"from":%d}""" from
    let actual = ToJson query
    Assert.AreEqual(expected, actual)
    
[<Property>]
let ``Size serializes correctly``(size) =
    let query =
        Search [
            Size size
        ]
    let expected = sprintf """{"size":%d}""" size
    let actual = ToJson query
    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``Source serializes correctly``() =
    let query =
        Search [
            Source_ Nothing
        ]
    let expected = """{"_source":false}"""
    let actual = ToJson query
    Assert.AreEqual(expected, actual)
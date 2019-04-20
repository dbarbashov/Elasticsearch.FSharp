module Elasticsearch.FSharp.Tests.Query

open NUnit.Framework
open FsCheck.NUnit

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization

[<Test>]
let ``"match_all" serializes correctly``() =
    let query =
        Search [
            Query MatchAll
        ]
    let expected = """{"query":{"match_all":{}}}"""
    let actual = ToJson query
    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``"match_none" serializes correctly``() =
    let query =
        Search [
            Query MatchNone
        ]
    let expected = """{"query":{"match_none":{}}}"""
    let actual = ToJson query
    Assert.AreEqual(expected, actual)
    
[<Property(MaxTest=10000)>]
let ``"exists" serialization works correctly``(fieldName) =
    let query =
        Search [
            Query(
                Exists fieldName
            )
        ]
    let expected = sprintf """{"query":{"exists":{"field":"%s"}}}""" fieldName
    let actual = ToJson query
    expected = actual
    
[<Property(MaxTest=10000)>]
let ``"raw" serialization works correctly``(rawQuery) =
    let query =
        Search [
            Query(
                Raw rawQuery
            )
        ]
    let expected = sprintf """{"query":{%s}}""" rawQuery
    let actual = ToJson query
    expected = actual
module Elasticsearch.FSharp.Tests.Queries.MultiMatchQueryTests

open NUnit.Framework
open FsCheck.NUnit
open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization
open Elasticsearch.FSharp.Utility
open Elasticsearch.FSharp.Tests.Helpers

[<Property>]
// TODO don't know how to specify range for tie_breaker value (by default values from -Infinity to Infinity are generated)
let ``"multi_match" base serializes correctly``(queryType, field, queryString, expansions, slop) = 
    let query =
        Search [
            Query (
                MultiMatch [
                    QueryType queryType
                    Fields [field]
                    MultiMatchQuery queryString
                    MaxExpansions expansions
                    Slop slop
                    TieBreaker 0.3
                ]
            )
        ]
    let expected = sprintf """{"query":{"multi_match":{"type":"%s","fields":["%s"],"query":"%s","max_expansions":%d,"slop":%d,"tie_breaker":0.3}}}"""
                       (Json.escapeString queryType) (Json.escapeString field) (Json.escapeString queryString) expansions slop
    let actual = toJson query
    Assert.AreEqual(expected, actual)

[<Test>]
let ``"multi_match" with raw string param serializes correctly``() =
    let query =
        Search [
            Query (
                MultiMatch [
                    MultiMatchQuery "search text"
                    MultiMatchQueryField.MultiMatchRaw ("custom_param", Json.quoteString "custom_value")
                ]
            )
        ]
    let expected = """{"query":{"multi_match":{"query":"search text","custom_param":"custom_value"}}}"""
    Assert.AreEqual(expected, (toJson query))

[<Test>]
let ``"multi_match" with raw numeric param serializes correctly``() =
    let query =
        Search [
            Query (
                MultiMatch [
                    MultiMatchQuery "search text"
                    MultiMatchQueryField.MultiMatchRaw ("custom_param", "123.45") // Value is a pre-formatted JSON number
                ]
            )
        ]
    let expected = """{"query":{"multi_match":{"query":"search text","custom_param":123.45}}}"""
    Assert.AreEqual(expected, (toJson query))

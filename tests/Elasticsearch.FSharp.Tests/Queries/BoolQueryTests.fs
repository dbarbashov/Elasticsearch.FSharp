module Elasticsearch.FSharp.Tests.Queries.BoolQueryTests

open NUnit.Framework
open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization
open Elasticsearch.FSharp.Utility
open Elasticsearch.FSharp.Tests.Helpers

[<Test>]
let ``"bool" with "must" serializes correctly``() =
    let query =
        Search [
            Query (
                Bool [
                    Must [
                        MatchAll
                    ]
                ]
            )
        ]
    let expected = """{"query":{"bool":{"must":[{"match_all":{}}]}}}"""
    let actual = toJson query
    Assert.AreEqual(expected, actual)

[<Test>]
let ``"bool" with "filter" serializes correctly``() =
    let query =
        Search [
            Query (
                Bool [
                    Filter [
                        Term ("field", [ExactValue "value"])
                    ]
                ]
            )
        ]
    let expected = """{"query":{"bool":{"filter":[{"term":{"field":{"value":"value"}}}]}}}"""
    let actual = toJson query
    Assert.AreEqual(expected, actual)

[<Test>]
let ``"bool" with "should" serializes correctly``() =
    let query =
        Search [
            Query (
                Bool [
                    Should [
                        Match ("field", [MatchQuery "value"])
                    ]
                ]
            )
        ]
    let expected = """{"query":{"bool":{"should":[{"match":{"field":{"query":"value"}}}]}}}"""
    let actual = toJson query
    Assert.AreEqual(expected, actual)

[<Test>]
let ``"bool" with "must_not" serializes correctly``() =
    let query =
        Search [
            Query (
                Bool [
                    MustNot [
                        Range ("field", [Gte "10"])
                    ]
                ]
            )
        ]
    let expected = """{"query":{"bool":{"must_not":[{"range":{"field":{"gte":"10"}}}]}}}"""
    let actual = toJson query
    Assert.AreEqual(expected, actual)

[<Test>]
let ``"bool" with "minimum_should_match" serializes correctly`` =
    let msm = "1.0"
    
    let query =
        Search [
            Query (
                Bool [
                    Should [ MatchAll ] // minimum_should_match typically requires a should clause
                    MinimumShouldMatch msm
                ]
            )
        ]
    let expected = sprintf """{"query":{"bool":{"should":[{"match_all":{}}],"minimum_should_match":"%s"}}}""" (Json.escapeString msm)
    let actual = toJson query
    Assert.AreEqual(expected, actual)

[<Test>]
let ``"bool" with multiple clauses serializes correctly``() =
    let query =
        Search [
            Query (
                Bool [
                    Must [ Match ("title", [MatchQuery "elasticsearch"]) ]
                    Filter [ Term ("status", [ExactValue "published"]) ]
                    MustNot [ Term ("tags", [ExactValue "archived"]) ]
                    Should [ Match ("content", [MatchQuery "relevant"]) ]
                    MinimumShouldMatch "1"
                ]
            )
        ]
    let expected = """{"query":{"bool":{"must":[{"match":{"title":{"query":"elasticsearch"}}}],"filter":[{"term":{"status":{"value":"published"}}}],"must_not":[{"term":{"tags":{"value":"archived"}}}],"should":[{"match":{"content":{"query":"relevant"}}}],"minimum_should_match":"1"}}}"""
    let actual = toJson query
    Assert.AreEqual(Helpers.removeWhitespace expected, Helpers.removeWhitespace actual)

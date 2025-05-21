module Elasticsearch.FSharp.Tests.Queries.MatchPhrasePrefixQueryTests

open NUnit.Framework
open FsCheck.NUnit
open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization
open Elasticsearch.FSharp.Utility

[<Property>]
let ``"match_phrase_prefix" base serializes correctly`` (fieldName, fieldValue, expansions) =
    let query =
        Search [
            Query (
                MatchPhrasePrefix (
                    fieldName,
                    [
                        MatchPhrasePrefixQueryField.MatchQuery fieldValue
                        MatchPhrasePrefixQueryField.MaxExpansions expansions
                    ]
                )
            )
        ]
    let expected = sprintf """{"query":{"match_phrase_prefix":{"%s":{"query":"%s","max_expansions":%d}}}}"""
                       (Json.escapeString fieldName) (Json.escapeString fieldValue) expansions
    let actual = toJson query
    Assert.AreEqual(expected, actual)

[<Property>]
let ``"match_phrase_prefix" with boost serializes correctly`` (fieldName, fieldValue, expansions : int, boostValue: float) =
    let boost = System.Math.Round(boostValue, 2) // Round to make test predictable with float ToString
    let query =
        Search [
            Query (
                MatchPhrasePrefix (
                    fieldName,
                    [
                        MatchPhrasePrefixQueryField.MatchQuery fieldValue
                        MatchPhrasePrefixQueryField.MaxExpansions expansions
                        MatchPhrasePrefixQueryField.Boost boost
                    ]
                )
            )
        ]
    let expected = sprintf """{"query":{"match_phrase_prefix":{"%s":{"query":"%s","max_expansions":%d,"boost":%s}}}}"""
                       (Json.escapeString fieldName) (Json.escapeString fieldValue) expansions (boost.ToString())
    let actual = toJson query
    Assert.AreEqual(expected, actual)

[<Property>]
let ``"match_phrase_prefix" with slop serializes correctly`` (fieldName, fieldValue, slopVal: int) =
    let query =
        Search [
            Query (
                MatchPhrasePrefix (
                    fieldName,
                    [
                        MatchPhrasePrefixQueryField.MatchQuery fieldValue
                        MatchPhrasePrefixQueryField.Slop slopVal
                    ]
                )
            )
        ]
    let expected = sprintf """{"query":{"match_phrase_prefix":{"%s":{"query":"%s","slop":%d}}}}"""
                       (Json.escapeString fieldName) (Json.escapeString fieldValue) slopVal
    Assert.AreEqual(expected, toJson query)

[<Property>]
let ``"match_phrase_prefix" with analyzer serializes correctly`` (fieldName, fieldValue, analyzerName) =
    let query =
        Search [
            Query (
                MatchPhrasePrefix (
                    fieldName,
                    [
                        MatchPhrasePrefixQueryField.MatchQuery fieldValue
                        MatchPhrasePrefixQueryField.Analyzer analyzerName
                    ]
                )
            )
        ]
    let expected = sprintf """{"query":{"match_phrase_prefix":{"%s":{"query":"%s","analyzer":"%s"}}}}"""
                       (Json.escapeString fieldName) (Json.escapeString fieldValue) (Json.escapeString analyzerName)
    Assert.AreEqual(expected, toJson query)

[<Test>]
let ``"match_phrase_prefix" with all fields serializes correctly`` () =
    let fieldName = "message"
    let fieldValue = "quick brown fox"
    let maxExp = 10
    let slopVal = 2
    let analyzerName = "standard"
    let boostVal = 1.5
    let query =
        Search [
            Query (
                MatchPhrasePrefix (
                    fieldName,
                    [
                        MatchPhrasePrefixQueryField.MatchQuery fieldValue
                        MatchPhrasePrefixQueryField.MaxExpansions maxExp
                        MatchPhrasePrefixQueryField.Slop slopVal
                        MatchPhrasePrefixQueryField.Analyzer analyzerName
                        MatchPhrasePrefixQueryField.Boost boostVal
                    ]
                )
            )
        ]
    let expected = sprintf """{"query":{"match_phrase_prefix":{"%s":{"query":"%s","max_expansions":%d,"slop":%d,"analyzer":"%s","boost":%s}}}}"""
                       (Json.escapeString fieldName) (Json.escapeString fieldValue) maxExp slopVal (Json.escapeString analyzerName) (boostVal.ToString())
    Assert.AreEqual(expected, toJson query)

module Elasticsearch.FSharp.Tests.Queries.WildcardQueryTests

open NUnit.Framework
open FsCheck.NUnit
open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization
open Elasticsearch.FSharp.Utility
open Elasticsearch.FSharp.Tests.Helpers

[<Property>]
let ``"wildcard" base serializes correctly``(fieldName, patternValue) =
    let query =
        Search [
            Query (
                Wildcard (fieldName, [PatternValue patternValue])
            )
        ]
    let expected = sprintf """{"query":{"wildcard":{"%s":{"value":"%s"}}}}"""
                       (Json.escapeString fieldName) (Json.escapeString patternValue)
    let actual = toJson query
    Assert.AreEqual(expected, actual)

[<Property>]
let ``"wildcard" with rewrite and boost serializes correctly``(fieldName, patternValue, boostValue: float) =
    let boost = System.Math.Round(boostValue, 2) // Round to make test predictable with float ToString
    let query =
        Search [
            Query (
                Wildcard (fieldName, [
                    PatternValue patternValue
                    WildcardQueryField.Rewrite RewriteOption.ConstantScore
                    WildcardQueryField.Boost boost
                ])
            )
        ]
    let expected = sprintf """{"query":{"wildcard":{"%s":{"value":"%s","rewrite":"constant_score","boost":%s}}}}"""
                       (Json.escapeString fieldName) (Json.escapeString patternValue) (boost.ToString())
    let actual = toJson query
    Assert.AreEqual(expected, actual)

[<Property>]
let ``"wildcard" with rewrite top_terms_N serializes correctly``(fieldName, patternValue, n:int) =
    let nVal = abs n % 100 + 1 // Ensure N is positive and reasonable for a test
    let query =
        Search [
            Query (
                Wildcard (fieldName, [
                    PatternValue patternValue
                    WildcardQueryField.Rewrite (RewriteOption.TopTerms nVal)
                ])
            )
        ]
    let expected = sprintf """{"query":{"wildcard":{"%s":{"value":"%s","rewrite":"top_terms_%d"}}}}"""
                       (Json.escapeString fieldName) (Json.escapeString patternValue) nVal
    let actual = toJson query
    Assert.AreEqual(expected, actual)

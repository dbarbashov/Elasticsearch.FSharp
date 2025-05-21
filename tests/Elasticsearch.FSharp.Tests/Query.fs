module Elasticsearch.FSharp.Tests.Query

open Elasticsearch.FSharp.Utility
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
    let actual = toJson query
    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``"match_none" serializes correctly``() =
    let query =
        Search [
            Query MatchNone
        ]
    let expected = """{"query":{"match_none":{}}}"""
    let actual = toJson query
    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``"ids" serializes correctly``() =
    let query =
        Search [
            Query (Ids ["foo"; "bar"])
        ]
    let expected = """{"query":{"ids":{"values":["foo","bar"]}}}"""
    let actual = toJson query
    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``"bool" serializes correctly``() =
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
    
[<Property>]
let ``"match" serializes correctly``(fieldName, fieldValue) = 
    let query =
        Search [
            Query (
                Match (fieldName, [MatchQuery fieldValue])
            )
        ]
    let expected = sprintf """{"query":{"match":{"%s":{"query":"%s"}}}}"""
                       (Json.escapeString fieldName) (Json.escapeString fieldValue)
    let actual = toJson query
    Assert.AreEqual(expected, actual)
    
[<Property>]
let ``"term" serializes correctly``(fieldName, fieldValue) = 
    let query =
        Search [
            Query (
                Term (fieldName, [ExactValue fieldValue])
            )
        ]
    let expected = sprintf """{"query":{"term":{"%s":{"value":"%s"}}}}"""
                       (Json.escapeString fieldName) (Json.escapeString fieldValue)
    let actual = toJson query
    Assert.AreEqual(expected, actual)
    
[<Property>]
let ``"terms" serializes correctly``(fieldName, fieldValue) = 
    let query =
        Search [
            Query (
                Terms (fieldName, [ValueList [fieldValue]])
            )
        ]
    let expected = sprintf """{"query":{"terms":{"%s":["%s"]}}}"""
                       (Json.escapeString fieldName) (Json.escapeString fieldValue)
    let actual = toJson query
    Assert.AreEqual(expected, actual)
  
[<Property>]
let ``"range" serializes correctly``(fieldName, fieldValue) = 
    let query =
        Search [
            Query (
                Range (fieldName, [Gte fieldValue])
            )
        ]
    let expected = sprintf """{"query":{"range":{"%s":{"gte":"%s"}}}}"""
                       (Json.escapeString fieldName) (Json.escapeString fieldValue)
    let actual = toJson query
    Assert.AreEqual(expected, actual)
    
[<Property>]
let ``"script" serializes correctly`` scriptName scriptSource =
    let query =
        Search [
            Query MatchAll
            ScriptFields [
                scriptName, [ ScriptField.Lang "painless"; ScriptField.Source scriptSource ]
            ]
        ]
    let expected = sprintf """{"query":{"match_all":{}},"script_fields":{"%s":{"script":{"lang":"painless","source":"%s"}}}}"""
                       (Json.escapeString scriptName) (Json.escapeString scriptSource)
    let actual = toJson query
    Assert.AreEqual(expected, actual)
    
[<Property>]
// TODO don't know how to specify range for tie_breaker value (by default values from -Infinity to Infinity are generated)
let ``"multi_match" serializes correctly``(queryType, field, queryString, expansions, slop) = 
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

[<Property>]
let ``"match_phrase_prefix* serializes correctly`` (fieldName, fieldValue, expansions) =
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
let ``"match_phrase_prefix" with rewrite and boost serializes correctly`` (fieldName, fieldValue, expansions : int, boostValue: float) =
    let boost = System.Math.Round(boostValue, 2) // Round to make test predictable with float ToString
    let query =
        Search [
            Query (
                MatchPhrasePrefix (
                    fieldName,
                    [
                        MatchPhrasePrefixQueryField.MatchQuery fieldValue
                        MatchPhrasePrefixQueryField.MaxExpansions expansions
                        MatchPhrasePrefixQueryField.Rewrite RewriteOption.ScoringBoolean
                        MatchPhrasePrefixQueryField.Boost boost
                    ]
                )
            )
        ]
    let expected = sprintf """{"query":{"match_phrase_prefix":{"%s":{"query":"%s","max_expansions":%d,"rewrite":"scoring_boolean","boost":%s}}}}"""
                       (Json.escapeString fieldName) (Json.escapeString fieldValue) expansions (boost.ToString("F1", System.Globalization.CultureInfo.InvariantCulture))
    let actual = toJson query
    Assert.AreEqual(expected, actual)

[<Property>]
let ``"match_phrase_prefix" with rewrite top_terms_boost_N serializes correctly`` (fieldName, fieldValue, expansions, n:int) =
    let nVal = abs n % 100 + 1
    let query =
        Search [
            Query (
                MatchPhrasePrefix (
                    fieldName,
                    [
                        MatchPhrasePrefixQueryField.MatchQuery fieldValue
                        MatchPhrasePrefixQueryField.MaxExpansions expansions
                        MatchPhrasePrefixQueryField.Rewrite (RewriteOption.TopTermsBoost nVal)
                    ]
                )
            )
        ]
    let expected = sprintf """{"query":{"match_phrase_prefix":{"%s":{"query":"%s","max_expansions":%d,"rewrite":"top_terms_boost_%d"}}}}"""
                       (Json.escapeString fieldName) (Json.escapeString fieldValue) expansions nVal
    let actual = toJson query
    Assert.AreEqual(expected, actual)

[<Property(MaxTest=10000)>]
let ``"exists" serialization works correctly``(fieldName) =
    let query =
        Search [
            Query(
                Exists fieldName
            )
        ]
    let expected = sprintf """{"query":{"exists":{"field":"%s"}}}""" (Json.escapeString fieldName)
    let actual = toJson query
    expected = actual
    
[<Property(MaxTest=10000)>]
let ``"raw" serialization works correctly``(rawQuery) =
    let query =
        Search [
            Query(
                QueryBody.Raw rawQuery
            )
        ]
    let expected = sprintf """{"query":{%s}}""" rawQuery
    let actual = toJson query
    expected = actual
    
[<Property>]
let ``"type" serializes correctly``(``type``) = 
    let query =
        Search [
            Query (
                TypeEquals ``type``
            )
        ]
    let expected = sprintf """{"query":{"type":{"value":"%s"}}}""" (Json.escapeString ``type``)
    let actual = toJson query
    Assert.AreEqual(expected, actual)

[<Property>]
let ``"wildcard" serializes correctly``(fieldName, patternValue) =
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
                       (Json.escapeString fieldName) (Json.escapeString patternValue) (boost.ToString("F1", System.Globalization.CultureInfo.InvariantCulture))
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

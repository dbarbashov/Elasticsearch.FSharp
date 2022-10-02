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
    let expected = sprintf """{"query":{"match":{"%s":{"query":"%s"}}}}""" fieldName fieldValue
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
    let expected = sprintf """{"query":{"term":{"%s":{"value":"%s"}}}}""" fieldName fieldValue
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
    let expected = sprintf """{"query":{"terms":{"%s":["%s"]}}}""" fieldName fieldValue
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
    let expected = sprintf """{"query":{"range":{"%s":{"gte":"%s"}}}}""" fieldName fieldValue
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
    let expected = sprintf """{"query":{"match_all":{}},"script_fields":{"%s":{"script":{"lang":"painless","source":"%s"}}}}""" scriptName scriptSource
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
    let expected = sprintf """{"query":{"multi_match":{"type":"%s","fields":["%s"],"query":"%s","max_expansions":%d,"slop":%d,"tie_breaker":0.3}}}""" queryType field queryString expansions slop
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
    let expected = sprintf """{"query":{"match_phrase_prefix":{"%s":{"query":"%s","max_expansions":%d}}}}""" fieldName fieldValue expansions
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
    let expected = sprintf """{"query":{"exists":{"field":"%s"}}}""" fieldName
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
    let expected = sprintf """{"query":{"type":{"value":"%s"}}}""" ``type``
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
    let expected = sprintf """{"query":{"wildcard":{"%s":{"value":"%s"}}}}""" fieldName patternValue
    let actual = toJson query
    Assert.AreEqual(expected, actual)

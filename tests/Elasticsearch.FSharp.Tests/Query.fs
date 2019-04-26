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
    let actual = ToJson query
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
    let actual = ToJson query
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
    let actual = ToJson query
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
    let actual = ToJson query
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
    let actual = ToJson query
    Assert.AreEqual(expected, actual)
    
[<Property>]
let ``"script" serializes correctly``(scriptSource) = 
    let query =
        Search [
            Query (
                Script [Lang "painless"; Source scriptSource]
            )
        ]
    let expected = sprintf """{"query":{"script":{"script":{"lang":"painless","source":"%s"}}}}""" scriptSource
    let actual = ToJson query
    Assert.AreEqual(expected, actual)
    
[<Property>]
let ``"multi_match" serializes correctly``(field, queryString) = 
    let query =
        Search [
            Query (
                MultiMatch [Fields [field]; MultiMatchQuery queryString]
            )
        ]
    let expected = sprintf """{"query":{"multi_match":{"fields":["%s"],"query":"%s"}}}""" field queryString
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
    
[<Property>]
let ``"type" serializes correctly``(``type``) = 
    let query =
        Search [
            Query (
                TypeEquals ``type``
            )
        ]
    let expected = sprintf """{"query":{"type":{"value":"%s"}}}""" ``type``
    let actual = ToJson query
    Assert.AreEqual(expected, actual)
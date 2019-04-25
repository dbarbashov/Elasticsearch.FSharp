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
    
[<Property(MaxTest=10000)>]
let ``ScriptFields serializes correctly``(script1, script2, fieldName1, fieldName2, paramName, paramValue) =
    let query =
        Search [
            ScriptFields [
                fieldName1, [
                    Lang "painless"
                    Source script1
                ]
                fieldName2, [
                    Lang "painless"
                    Source script2
                    Params [
                        paramName, paramValue
                    ]
                ]
            ]
        ]
    let expected =
        sprintf
            """{"script_fields":{"%s":{"script":{"lang":"painless","source":"%s"}},"%s":{"script":{"lang":"painless","source":"%s","params":{"%s":"%s"}}}}}"""
            fieldName1
            script1
            fieldName2
            script2
            paramName
            paramValue
            
    let actual = ToJson query
    Assert.AreEqual(expected, actual)
    
[<Property(MaxTest=10000)>]
let ``Aggs serializes correctly``(aggName, aggFieldName) =
    let query =
        Search [
            Aggs [
                NamedAgg (
                    aggName, Avg [
                        AggField aggFieldName
                    ]
                )
            ]
        ]
    let expected = sprintf """{"aggs":{"%s":{"avg":{"field":"%s"}}}}""" aggName aggFieldName
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
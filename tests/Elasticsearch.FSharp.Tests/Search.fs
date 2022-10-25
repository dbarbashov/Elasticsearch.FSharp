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
    let actual = toJson query
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
    let actual = toJson query
    Assert.AreEqual(expected, actual)
    
[<Property(MaxTest=10000)>]
let ``ScriptFields serializes correctly``(script1, script2, fieldName1, fieldName2, paramName, paramValue) =
    let query =
        Search [
            SearchBody.ScriptFields [
                fieldName1, [
                    ScriptField.Lang "painless"
                    ScriptField.Source script1
                ]
                fieldName2, [
                    ScriptField.Lang "painless"
                    ScriptField.Source script2
                    ScriptField.Params [
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
            
    let actual = toJson query
    Assert.AreEqual(expected, actual)
    
[<Property(MaxTest=10000)>]
let ``Script serializes correctly``(script1, paramName, paramValue) =
    let query =
        Search [
            Query (
                Bool [
                    Filter [
                        Script [
                            Script.Lang "painless"
                            Script.Source script1
                            Script.Params [
                                paramName, paramValue
                            ]
                        ]
                   ]
                ]
            )
        ]

    let expected =
        sprintf
            """{"query":{"bool":{"filter":[{"script":{"script":{"lang":"painless","source":"%s","params":{"%s":"%s"}}}}]}}}"""
            script1
            paramName
            paramValue
            
    let actual = toJson query
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
    let actual = toJson query
    Assert.AreEqual(expected, actual)

[<Property>]
let ``Weighted agg serializes correctly``(aggName, aggFieldName, aggValueField) =
    let query =
        Search [
            Aggs [
                NamedAgg (
                    aggName, WeightedAvg [
                        AggWeight (WeightValueField aggValueField)
                        AggWeight (WeightField aggFieldName)
                    ]
                )
            ]
        ]
    let expected = sprintf """{"aggs":{"%s":{"weighted_avg":{"value":{"field":"%s"},"weight":{"field":"%s"}}}}}"""
                       aggName aggValueField aggFieldName
    let actual = toJson query
    Assert.AreEqual(expected, actual)

[<Property>]
let ``Value count aggregation serializes correctly``(aggName, aggFieldName) =
    let expected = sprintf """{"aggs":{"%s":{"value_count":{"field":"%s"}}}}""" aggName aggFieldName
    let actual =
        Search [
            Aggs [
                NamedAgg (
                    aggName, ValueCount [
                        AggField aggFieldName
                    ]
                )
            ]
        ]
        |> toJson
    Assert.AreEqual(expected, actual)

[<Property(MaxTest=10000)>]
let ``Complex aggs serializes correctly``(complexAggName, complexAggFieldName,
                                         complexFilterAggName, complexFilterAggField,
                                         simpleAggName, simpleAggField) =
    let query =
        Search [
            Aggs [
                NamedComplexAgg (
                    complexAggName,
                    Avg [ AggField complexAggFieldName],
                    [
                        FilterComplexAgg (
                            complexFilterAggName,
                            MatchAll,
                            Avg [ AggField complexFilterAggField ],
                            [
                                NamedAgg (
                                    simpleAggName, Avg [ AggField simpleAggField ]
                                )
                            ]
                        )
                    ]
                )
            ]
        ]
    let expected = sprintf """{"aggs":{"%s":{"avg":{"field":"%s"},"aggs":{"%s":{"filter":{"match_all":{}},"aggs":{"%s":{"avg":{"field":"%s"},"aggs":{"%s":{"avg":{"field":"%s"}}}}}}}}}}"""
                       complexAggName complexAggFieldName complexFilterAggName complexFilterAggName complexFilterAggField simpleAggName simpleAggField
    let actual = toJson query
    Assert.AreEqual(expected, actual)
    
[<Property>]
let ``From serializes correctly``(from) =
    let query =
        Search [
            From from
        ]
    let expected = sprintf """{"from":%d}""" from
    let actual = toJson query
    Assert.AreEqual(expected, actual)
    
[<Property>]
let ``Size serializes correctly``(size) =
    let query =
        Search [
            Size size
        ]
    let expected = sprintf """{"size":%d}""" size
    let actual = toJson query
    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``Source serializes correctly``() =
    let query =
        Search [
            Source_ Nothing
        ]
    let expected = """{"_source":false}"""
    let actual = toJson query
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Raw serialized correctly``() =
    let query =
        Search [
            SearchBody.Raw ("query", """{"match_all":{}}""")
        ]
    let expected = """{"query":{"match_all":{}}}"""
    let actual = toJson query
    Assert.AreEqual(expected, actual)
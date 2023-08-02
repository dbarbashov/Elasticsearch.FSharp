module Elasticsearch.FSharp.Tests.Sort

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization
open Elasticsearch.FSharp.Utility
open NUnit.Framework
open FsCheck.NUnit

[<Property(MaxTest=10000)>]
let ``Sort serializes correctly``(fieldName, sortOrder, sortMode) =
    let query =
        Search [
            Sort [fieldName, [Order sortOrder; Mode sortMode]]
            Query MatchAll
        ]
        
    let orderStr =
        match sortOrder with
        | SortOrder.Asc -> "asc"
        | SortOrder.Desc -> "desc"
        
    let modeStr =
        match sortMode with
        | SortMode.Min -> "min"
        | SortMode.Max -> "max"
        | SortMode.Sum -> "sum"
        | SortMode.Avg -> "avg"
        | SortMode.Median -> "median"
        
    let expected =
        sprintf """{"sort":[{"%s":{"order":"%s","mode":"%s"}}],"query":{"match_all":{}}}"""
            (Json.escapeString fieldName) (Json.escapeString orderStr) (Json.escapeString modeStr)
    let actual = toJson query
    Assert.AreEqual(expected, actual)
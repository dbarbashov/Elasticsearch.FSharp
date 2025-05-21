module Elasticsearch.FSharp.Tests.Queries.ExistsQueryTests

open NUnit.Framework
open FsCheck.NUnit
open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization
open Elasticsearch.FSharp.Utility
open Elasticsearch.FSharp.Tests.Helpers

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
    Assert.AreEqual(expected, actual)

module Elasticsearch.FSharp.Tests.Queries.TypeQueryTests

open NUnit.Framework
open FsCheck.NUnit
open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization
open Elasticsearch.FSharp.Utility
open Elasticsearch.FSharp.Tests.Helpers

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

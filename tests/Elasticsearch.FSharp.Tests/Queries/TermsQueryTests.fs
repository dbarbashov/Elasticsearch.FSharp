module Elasticsearch.FSharp.Tests.Queries.TermsQueryTests

open NUnit.Framework
open FsCheck.NUnit
open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization
open Elasticsearch.FSharp.Utility
open Elasticsearch.FSharp.Tests.Helpers

[<Property>]
let ``"terms" with value list serializes correctly``(fieldName, fieldValue) =
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
let ``"terms" with lookup serializes correctly``(fieldName, index, esType, id, path) =
    let query =
        Search [
            Query (
                Terms (fieldName, [FromIndex (index, esType, id, path)])
            )
        ]
    let expected = sprintf """{"query":{"terms":{"%s":{"index":"%s","type":"%s","id":"%s","path":"%s"}}}}"""
                       (Json.escapeString fieldName) (Json.escapeString index) (Json.escapeString esType) (Json.escapeString id) (Json.escapeString path)
    let actual = toJson query
    Assert.AreEqual(expected, actual)

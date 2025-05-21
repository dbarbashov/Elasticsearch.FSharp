module Elasticsearch.FSharp.Tests.SearchBody.ScriptFieldsTests

open NUnit.Framework
open FsCheck.NUnit
open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization
open Elasticsearch.FSharp.Utility
open Elasticsearch.FSharp.Tests.Helpers

[<Property>]
let ``"script_fields" in search body serializes correctly`` scriptName scriptSource =
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

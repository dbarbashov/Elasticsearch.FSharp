module Elasticsearch.FSharp.Tests.Queries.QueryBodyScriptTests

open NUnit.Framework
open FsCheck.NUnit
open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization
open Elasticsearch.FSharp.Utility
open Elasticsearch.FSharp.Tests.Helpers

[<Property>]
let ``"script" query serializes correctly``(scriptSource, lang) =
    let query =
        Search [
            Query (
                QueryBody.Script [ 
                    Script.Source scriptSource
                    Script.Lang lang
                ]
            )
        ]
    let expected = sprintf """{"query":{"script":{"script":{"source":"%s","lang":"%s"}}}}"""
                       (Json.escapeString scriptSource) (Json.escapeString lang)
    let actual = toJson query
    Assert.AreEqual(expected, actual)

[<Property>]
let ``"script" query with params serializes correctly``(scriptSource, lang, pName, pValue) =
    let query =
        Search [
            Query (
                QueryBody.Script [ 
                    Script.Source scriptSource
                    Script.Lang lang
                    Script.Params [pName, pValue]
                ]
            )
        ]
    let expected = sprintf """{"query":{"script":{"script":{"source":"%s","lang":"%s","params":{"%s":"%s"}}}}}"""
                       (Json.escapeString scriptSource) (Json.escapeString lang) (Json.escapeString pName) (Json.escapeString pValue)
    let actual = toJson query
    Assert.AreEqual(expected, actual)

module Elasticsearch.FSharp.Tests.Queries.ScriptQuery

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization
open FsCheck.NUnit

[<Property(MaxTest=10000)>]
let ``"script fields" query serializes correctly`` scriptSource scriptLang =
    let query =
        Search [
            Query (
                ScriptFields [
                    ScriptField.Source scriptSource
                    ScriptField.Lang scriptLang
                ]
            )
        ]
    let expected = sprintf """{"query":{"script":{"script":{"source":"%s","lang":"%s"}}}}""" scriptSource scriptLang
    let actual = toJson query
    expected = actual
    
[<Property(MaxTest=10000)>]
let ``"script" query serializes correctly`` scriptSource scriptLang =
    let query =
        Search [
            Query (
                Script [
                    Script.Source scriptSource
                    Script.Lang scriptLang
                ]
            )
        ]
    let expected = sprintf """{"query":{"script":{"script":{"source":"%s","lang":"%s"}}}}""" scriptSource scriptLang
    let actual = toJson query
    expected = actual
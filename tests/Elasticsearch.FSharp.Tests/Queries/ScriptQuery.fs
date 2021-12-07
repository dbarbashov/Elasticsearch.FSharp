module Elasticsearch.FSharp.Tests.Queries.ScriptQuery

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization
open FsCheck.NUnit

[<Property(MaxTest=10000)>]
let ``"script fields" in search query serializes correctly`` scriptName scriptSource scriptLang =
    let query =
        Search [
            Query MatchAll
            ScriptFields [
                scriptName, [
                    ScriptField.Source scriptSource
                    ScriptField.Lang scriptLang
                ]
            ]
        ]
    let expected = sprintf """{"query":{"match_all":{}},"script_fields":{"%s":{"script":{"source":"%s","lang":"%s"}}}}""" scriptName scriptSource scriptLang
    let actual = toJson query
    expected = actual
    
[<Property(MaxTest=10000)>]
let ``"script" in search query serializes correctly`` scriptSource scriptLang =
    let query =
        Search [
            Query MatchAll
            SearchBody.Script [
                Script.Source scriptSource
                Script.Lang scriptLang
            ]
        ]
    let expected = sprintf """{"query":{"match_all":{}},"script":{"source":"%s","lang":"%s"}}""" scriptSource scriptLang
    let actual = toJson query
    expected = actual
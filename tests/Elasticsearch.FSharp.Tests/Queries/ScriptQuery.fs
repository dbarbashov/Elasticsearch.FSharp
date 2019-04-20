module Elasticsearch.FSharp.Tests.Queries.ScriptQuery

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization
open FsCheck.NUnit

[<Property(MaxTest=10000)>]
let ``"script" query serializes correctly`` scriptSource scriptLang =
    let query =
        Search [
            Query (
                Script [
                    Source scriptSource
                    Lang scriptLang
                ]
            )
        ]
    let expected = sprintf """{"query":{"script":{"script":{"source":"%s","lang":"%s"}}}}""" scriptSource scriptLang
    let actual = ToJson query
    expected = actual
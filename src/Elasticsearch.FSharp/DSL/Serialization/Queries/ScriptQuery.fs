module Elasticsearch.FSharp.DSL.Serialization.Queries.ScriptQuery

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization.Script
open Elasticsearch.FSharp.Utility
    
let scriptQueryToJson (scriptBody: ScriptBody) =
    Json.makeObject [ scriptToJson scriptBody ]
module Elasticsearch.FSharp.DSL.Serialization.Queries.ScriptFieldsQuery

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization.ScriptField
open Elasticsearch.FSharp.Utility
    
let scriptQueryToJson (scriptBody: ScriptField list) =
    Json.makeObject [
        Json.makeKeyValue "script" (scriptFieldsToJson scriptBody)
    ]
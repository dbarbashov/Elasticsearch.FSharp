module Elasticsearch.FSharp.DSL.Serialization.Queries.ScriptQuery

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization.Script
    
let ScriptQueryToJson (scriptBody:ScriptField list) = 
    "{" + 
        (
            "\"script\":{" + (scriptFieldsToJson scriptBody) + "}"
        )
    + "}"
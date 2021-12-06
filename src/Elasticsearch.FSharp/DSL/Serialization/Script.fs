module internal Elasticsearch.FSharp.DSL.Serialization.Script

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.Utility
    
type Script with
    member x.ToJson() =
        match x with 
        | Script.Source x ->
            Json.makeKeyValue "source" (Json.quoteString x)
        | Script.Lang x -> 
            Json.makeKeyValue "lang" (Json.quoteString x)       
        | Script.Params x -> 
            Json.makeKeyValue "params" (Json.makeObject [
                for k, v in x ->
                    Json.makeKeyValue k (Json.quoteString v)
            ])
    
let scriptToJson (scriptBody: ScriptBody) =
    // elastic goes like: "script": { "script": { ... } }
    Json.makeObject [ Json.makeKeyValue "script" (Json.makeObject [
        for rangeParam in scriptBody ->
            rangeParam.ToJson()
    ]) ]
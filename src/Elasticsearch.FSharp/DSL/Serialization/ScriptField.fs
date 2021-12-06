module internal Elasticsearch.FSharp.DSL.Serialization.ScriptField

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.Utility
    
type ScriptField with
    member x.ToJson() =
        match x with 
        | ScriptField.Source x ->
            Json.makeKeyValue "source" (Json.quoteString x)
        | ScriptField.Lang x -> 
            Json.makeKeyValue "lang" (Json.quoteString x)
        | ScriptField.ScriptId x -> 
            Json.makeKeyValue "id" (Json.quoteString x)
        | ScriptField.Params x -> 
            Json.makeKeyValue "params" (Json.makeObject [
                for k, v in x ->
                    Json.makeKeyValue k (Json.quoteString v)
            ])
    
let scriptFieldsToJson (scriptBody: ScriptField list) =
    Json.makeObject [
        for rangeParam in scriptBody ->
            rangeParam.ToJson()
    ]

let scriptToJson ((name, scriptBody): ScriptFieldsBody) =
    Json.makeKeyValue name (Json.makeObject [
        Json.makeKeyValue "script" (scriptFieldsToJson scriptBody)
    ])
    
let scriptFieldsBodyToJson (fields: ScriptFieldsBody list) =
    Json.makeObject [
        for field in fields ->
            scriptToJson field
    ]
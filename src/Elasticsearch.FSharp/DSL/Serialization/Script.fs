module internal Elasticsearch.FSharp.DSL.Serialization.Script

open Elasticsearch.FSharp.DSL
    
let ScriptFieldsToJSON (scriptBody:ScriptField list) =
    ([
        for rangeParam in scriptBody ->
            match rangeParam with 
            | Source x -> 
                "\"source\":\"" + x + "\""
            | Lang x -> 
                "\"lang\":\"" + x + "\""
            | ScriptId x -> 
                "\"id\":\"" + x + "\""
            | Params x -> 
                let x = x |> List.map (fun (k, v) -> "\"" + k + "\":" + "\"" + v + "\"") |> String.concat ","
                "\"params\":{" + x + "}"
    ] |> String.concat ",") 

let ScriptToJson ((name, scriptBody): ScriptFieldsBody) =
    "\"" + name + "\":{\"script\":{" + (ScriptFieldsToJSON scriptBody) + "}}"
    
let ScriptFieldsBodyToJSON (fields: ScriptFieldsBody list) =
    [
        for field in fields ->
            ScriptToJson field
    ] |> String.concat ","
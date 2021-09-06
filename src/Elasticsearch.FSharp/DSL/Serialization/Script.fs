module internal Elasticsearch.FSharp.DSL.Serialization.Script

open Elasticsearch.FSharp.DSL
    
let scriptFieldsToJson (scriptBody:ScriptField list) =
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

let scriptToJson ((name, scriptBody): ScriptFieldsBody) =
    "\"" + name + "\":{\"script\":{" + (scriptFieldsToJson scriptBody) + "}}"
    
let scriptFieldsBodyToJson (fields: ScriptFieldsBody list) =
    [
        for field in fields ->
            scriptToJson field
    ] |> String.concat ","
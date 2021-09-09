module internal Elasticsearch.FSharp.DSL.Serialization.Queries.TermQuery

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.Utility

type TermQueryField with
    member x.ToJson() =
        match x with 
        | ExactValue null ->
            Json.makeObject [ Json.makeKeyValue "value" (Json.quoteString "") ]
        | ExactValue x ->
            Json.makeObject [ Json.makeKeyValue "value" (Json.quoteString x) ]
        
let termQueryToJson ((name, termBody) : string * (TermQueryField list) ) =
    Json.makeObject [
        Json.makeKeyValue name (
            match termBody with
            | [ termParam ] -> termParam.ToJson()
            | [] -> Json.makeObject []
            | _ -> failwithf "Illegal term query body %s: %A" name termBody 
        )
    ]
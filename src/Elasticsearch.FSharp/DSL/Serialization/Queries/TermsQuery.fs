module Elasticsearch.FSharp.DSL.Serialization.Queries.TermsQuery

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.Utility
    
type TermsQueryField with
    member x.ToJson() =
        match x with 
        | ValueList x ->
            Json.makeQuotedArray x
        | FromIndex (index, ``type``, id, path) ->
            Json.makeObject [
                Json.makeKeyValue "index" (Json.quoteString index)
                Json.makeKeyValue "type" (Json.quoteString ``type``)
                Json.makeKeyValue "id" (Json.quoteString id)
                Json.makeKeyValue "path" (Json.quoteString path)
            ]

let termsQueryToJson ((name, termsBody) : string * (TermsQueryField list)) =
    Json.makeObject [
        Json.makeKeyValue name (
            match termsBody with
            | [ body ] -> body.ToJson()
            | _ -> failwithf "Illegal terms body %s: %A" name termsBody
        )
    ]
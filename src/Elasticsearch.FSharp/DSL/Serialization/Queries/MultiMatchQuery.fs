module Elasticsearch.FSharp.DSL.Serialization.Queries.MultiMatchQuery

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.Utility
    
type MultiMatchQueryField with
    member x.ToJson() =
        match x with 
        | QueryType queryType ->
            Json.makeKeyValue "type" (Json.quoteString queryType)
        | Fields fieldList ->
            Json.makeKeyValue "fields" (Json.makeQuotedArray fieldList)
        | MultiMatchQuery null ->
            Json.makeKeyValue "query" (Json.quoteString "")
        | MultiMatchQuery query ->
            Json.makeKeyValue "query" (Json.quoteString query)
        | MultiMatchQueryField.MaxExpansions cnt ->
            Json.makeKeyValue "max_expansions" (cnt.ToString())
        | MultiMatchQueryField.Slop cnt ->
            Json.makeKeyValue "slop" (cnt.ToString())
        | TieBreaker tie ->
            Json.makeKeyValue "tie_breaker" (tie.ToString())
        | MultiMatchQueryField.MultiMatchRaw (key, value) ->
            Json.makeKeyValue key value
        
let multimatchBodyToJson (multimatchBody: MultiMatchQueryField list) =
    Json.makeObject [
        for field in multimatchBody ->
            field.ToJson()
    ]
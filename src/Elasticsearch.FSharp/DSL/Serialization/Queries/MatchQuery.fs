module Elasticsearch.FSharp.DSL.Serialization.Queries.MatchQuery

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.Utility

type MatchQueryField with
    member x.ToJson() =
        match x with 
        | Operator x ->
            Json.makeKeyValue "operator" (Json.quoteString x)
        | MatchQueryField.MatchQuery null ->
            Json.makeKeyValue "query" (Json.quoteString "")
        | MatchQueryField.MatchQuery x ->
            Json.makeKeyValue "query" (Json.quoteString x)
        | CutoffFrequency x ->
            Json.makeKeyValue "cutoff_frequency" (x.ToString())
        | ZeroTermsQuery x -> 
            Json.makeKeyValue "zero_terms_query" (x.ToString())

let matchQueryToJson ((name, matchBody) : string * (MatchQueryField list) ) =
    Json.makeObject [
        Json.makeKeyValue name (Json.makeObject [
            for matchParam in matchBody ->
                matchParam.ToJson()
        ])
    ]
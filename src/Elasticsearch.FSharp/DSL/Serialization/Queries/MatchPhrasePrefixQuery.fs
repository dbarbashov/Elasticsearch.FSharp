module Elasticsearch.FSharp.DSL.Serialization.Queries.MatchPhrasePrefixQuery
open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.Utility

type MatchPhrasePrefixQueryField with
    member x.ToJson() =
        match x with 
        | MatchPhrasePrefixQueryField.MatchQuery null ->
            Json.makeKeyValue "query" (Json.quoteString "")
        | MatchPhrasePrefixQueryField.MatchQuery x -> 
            Json.makeKeyValue "query" (Json.quoteString x)
        | MatchPhrasePrefixQueryField.MaxExpansions x ->
            Json.makeKeyValue "max_expansions" (x.ToString())

let matchPhrasePrefixQueryToJson ((name, matchBody) : string * (MatchPhrasePrefixQueryField list)) =
    Json.makeObject [
        Json.makeKeyValue name (Json.makeObject [
            for matchParam in matchBody ->
                matchParam.ToJson()
        ])
    ]
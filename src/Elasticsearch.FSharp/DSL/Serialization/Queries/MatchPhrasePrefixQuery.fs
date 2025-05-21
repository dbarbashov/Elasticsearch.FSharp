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
        | MatchPhrasePrefixQueryField.Slop x ->
            Json.makeKeyValue "slop" (x.ToString())
        | MatchPhrasePrefixQueryField.Analyzer x ->
            Json.makeKeyValue "analyzer" (Json.quoteString x)
        | MatchPhrasePrefixQueryField.Rewrite opt ->
            Json.makeKeyValue "rewrite" (Json.quoteString (opt.ToStringValue()))
        | MatchPhrasePrefixQueryField.Boost b ->
            Json.makeKeyValue "boost" (b.ToString())

let matchPhrasePrefixQueryToJson ((name, matchBody) : string * (MatchPhrasePrefixQueryField list)) =
    Json.makeObject [
        Json.makeKeyValue name (Json.makeObject [
            for matchParam in matchBody ->
                matchParam.ToJson()
        ])
    ]

module Elasticsearch.FSharp.DSL.Serialization.Queries.RangeQuery

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.Utility

type RangeQueryField with
    member x.ToJson() =
        match x with 
        | Gte x ->
            Json.makeKeyValue "gte" (Json.quoteString x)
        | Gt x -> 
            Json.makeKeyValue "gt" (Json.quoteString x)
        | Lte x -> 
            Json.makeKeyValue "lte" (Json.quoteString x)
        | Lt x -> 
            Json.makeKeyValue "lt" (Json.quoteString x)
        | RangeTimeZone x -> 
            Json.makeKeyValue "time_zone" (Json.quoteString x)
    
let rangeQueryToJson ((name, rangeBody): string * (RangeQueryField list) ) =
    Json.makeObject [
        Json.makeKeyValue name (Json.makeObject [
            for rangeParam in rangeBody ->
                rangeParam.ToJson()
        ])
    ]
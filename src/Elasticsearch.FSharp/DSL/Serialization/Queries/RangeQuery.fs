module Elasticsearch.FSharp.DSL.Serialization.Queries.RangeQuery

open Elasticsearch.FSharp.DSL
    
let rangeQueryToJson ((name, rangeBody): string * (RangeQueryField list) ) = 
    "{" + 
        (
            "\"" + name + "\":{" + 
                ([
                    for rangeParam in rangeBody ->
                        match rangeParam with 
                        | Gte x -> 
                            "\"gte\":\"" + x + "\""
                        | Gt x -> 
                            "\"gt\":\"" + x + "\""
                        | Lte x -> 
                            "\"lte\":\"" + x + "\""
                        | Lt x -> 
                            "\"lt\":\"" + x + "\""
                        | RangeTimeZone x -> 
                            "\"time_zone\":\"" + x + "\""
                ] |> String.concat ",")
            + "}"
        )
    + "}"
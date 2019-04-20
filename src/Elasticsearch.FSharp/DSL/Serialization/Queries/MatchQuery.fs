module Elasticsearch.FSharp.DSL.Serialization.Queries.MatchQuery

open Elasticsearch.FSharp.DSL

let MatchQueryToJson ((name, matchBody) : string * (MatchQueryField list) ) = 
    "{" + 
        (
            "\"" + name + "\":{" + 
                ([
                    for matchParam in matchBody ->
                        match matchParam with 
                        | Operator x -> 
                            "\"operator\":\"" + x + "\""
                        | MatchQuery x ->
                            if x = null then 
                                "\"query\":null"
                            else 
                                "\"query\":\"" + x + "\""
                        | CutoffFrequency x -> 
                            "\"cutoff_frequency\":" + x.ToString()
                        | ZeroTermsQuery x -> 
                            "\"zero_terms_query\":" + x.ToString()
                ] |> String.concat ",")
            + "}"
        )
    + "}"

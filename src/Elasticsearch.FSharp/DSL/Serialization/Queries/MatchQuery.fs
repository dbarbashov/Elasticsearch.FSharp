module Elasticsearch.FSharp.DSL.Serialization.Queries.MatchQuery

open Elasticsearch.FSharp.DSL

let matchQueryToJson ((name, matchBody) : string * (MatchQueryField list) ) = 
    "{" + 
        (
            "\"" + name + "\":{" + 
                ([
                    for matchParam in matchBody ->
                        match matchParam with 
                        | Operator x -> 
                            "\"operator\":\"" + x + "\""
                        | MatchQueryField.MatchQuery null -> "\"query\":\"\"" // TODO: fail here?
                        | MatchQueryField.MatchQuery x -> "\"query\":\"" + x + "\""
                        | CutoffFrequency x -> 
                            "\"cutoff_frequency\":" + x.ToString()
                        | ZeroTermsQuery x -> 
                            "\"zero_terms_query\":" + x.ToString()
                ] |> String.concat ",")
            + "}"
        )
    + "}"

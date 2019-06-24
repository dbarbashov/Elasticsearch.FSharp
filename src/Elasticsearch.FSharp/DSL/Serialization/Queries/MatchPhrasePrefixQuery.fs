module Elasticsearch.FSharp.DSL.Serialization.Queries.MatchPhrasePrefixQuery
open Elasticsearch.FSharp.DSL

let MatchPhrasePrefixQueryToJson ((name, matchBody) : string * (MatchPhrasePrefixQueryField list)) =
    "{" + 
        (
            "\"" + name + "\":{" + 
                ([
                    for matchParam in matchBody ->
                        match matchParam with 
                        | MatchPhrasePrefixQueryField.MatchQuery null -> "\"query\":\"\"" // TODO: fail here?
                        | MatchPhrasePrefixQueryField.MatchQuery x -> "\"query\":\"" + x + "\""
                        | MatchPhrasePrefixQueryField.MaxExpansions x -> "\"max_expansions\":" + x.ToString()
                        
                ] |> String.concat ",")
            + "}"
        )
    + "}"
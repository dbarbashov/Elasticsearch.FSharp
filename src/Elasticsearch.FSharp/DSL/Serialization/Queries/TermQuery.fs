module internal Elasticsearch.FSharp.DSL.Serialization.Queries.TermQuery

open Elasticsearch.FSharp.DSL

let TermQueryToJson ((name, termBody) : string * (TermQueryField list) ) = 
    "{" + 
        (
            "\"" + name + "\":" + 
                ([
                    for termParam in termBody->
                        match termParam with 
                        | ExactValue null -> "{\"value\":\"\"}" // TODO: fail here?
                        | ExactValue x -> "{\"value\":\"" + x + "\"}"
                ] |> String.concat ",")
        )
    + "}"
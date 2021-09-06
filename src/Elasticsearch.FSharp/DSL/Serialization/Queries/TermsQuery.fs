module Elasticsearch.FSharp.DSL.Serialization.Queries.TermsQuery

open Elasticsearch.FSharp.DSL
    
let termsQueryToJson ((name, termsBody) : string * (TermsQueryField list)) =     
    "{" + 
        (
            "\"" + name + "\":" + 
                (
                    match termsBody with 
                    | [ ValueList x ] -> 
                        "[" + (x |> List.map (fun x -> "\"" + x + "\"") |> String.concat ",") + "]"
                    | [ FromIndex (index, _type, id, path) ] -> 
                        "{" + 
                        "\"index\":\"" + index + "\"," +
                        "\"type\":\"" + _type + "\"," +
                        "\"id\":\"" + id + "\"," +
                        "\"path\":\"" + path+ "\"" +
                        "}" 
                    | _ ->
                        "_error_"
                )
        )
    + "}"
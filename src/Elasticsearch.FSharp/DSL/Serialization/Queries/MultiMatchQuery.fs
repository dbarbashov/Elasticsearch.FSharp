module Elasticsearch.FSharp.DSL.Serialization.Queries.MultiMatchQuery

open Elasticsearch.FSharp.DSL
    
let multimatchBodyToJson multimatchBody =
    "{" +
        ([
             for field in multimatchBody ->
                 match field with
                 | QueryType queryType -> "\"type\":\"" + queryType + "\""
                 | Fields fieldList ->
                     ("\"fields\":[" + (List.map (fun s -> "\"" + s + "\"") fieldList |> String.concat ",") + "]")
                 | MultiMatchQuery null -> "\"query\":\"\"" // TODO: fail here?
                 | MultiMatchQuery query -> "\"query\":\"" + query + "\""
                 | MultiMatchQueryField.MaxExpansions cnt -> """"max_expansions":""" + cnt.ToString()
                 | Slop cnt -> """"slop":""" + cnt.ToString()
                 | TieBreaker tie -> """"tie_breaker":""" + tie.ToString()
                 | MultiMatchQueryField.MultiMatchRaw (key, value) -> "\""+key+"\":" + value
        ] |> String.concat ",")
    + "}"
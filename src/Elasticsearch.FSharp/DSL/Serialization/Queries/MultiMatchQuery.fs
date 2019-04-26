module Elasticsearch.FSharp.DSL.Serialization.Queries.MultiMatchQuery

open Elasticsearch.FSharp.DSL
    
let MultimatchBodyToJson multimatchBody =
    "{" +
        ([
             for field in multimatchBody ->
                 match field with
                 | Fields fieldList ->
                     ("\"fields\":[" + (List.map (fun s -> "\"" + s + "\"") fieldList |> String.concat ",") + "]")
                 | MultiMatchQuery null -> "\"query\":\"\"" // TODO: fail here?
                 | MultiMatchQuery query -> "\"query\":\"" + query + "\""
        ] |> String.concat ",")
    + "}"
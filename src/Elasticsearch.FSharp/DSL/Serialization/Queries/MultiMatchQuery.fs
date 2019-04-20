module Elasticsearch.FSharp.DSL.Serialization.Queries.MultiMatchQuery

open Elasticsearch.FSharp.DSL
    
let MultimatchBodyToJson multimatchBody =
    "{" +
        ([
             for field in multimatchBody do
                 match field with
                 | Fields fieldList ->
                     yield ("\"fields\":[" + (List.map (fun s -> "\"" + s + "\"") fieldList |> String.concat ",") + "]")
                 | MultiMatchQuery query ->
                     if query = null then
                         yield "\"query\":null"
                     else 
                         yield "\"query\":\"" + query + "\""
        ] |> String.concat ",")
    + "}"
module Elasticsearch.FSharp.DSL.Serialization.Queries.WildcardQuery

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.Utility

type WildcardQueryField with
    member x.ToJson() =
        match x with
        | PatternValue null ->
            Json.makeObject [ Json.makeKeyValue "value" (Json.quoteString "") ]
        | PatternValue x ->
            Json.makeObject [ Json.makeKeyValue "value" (Json.quoteString x) ]

let wildcardQueryToJson ((name, wildcardBody) : string * (WildcardQueryField list) ) =
    Json.makeObject [
        Json.makeKeyValue name (
            match wildcardBody with
            | [ wildcardBodyParam ] -> wildcardBodyParam.ToJson()
            | [] -> Json.makeObject []
            | _ -> failwithf $"Illegal wildcard query body {name}: %A{wildcardBody}"
        )
    ]

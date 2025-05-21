module Elasticsearch.FSharp.DSL.Serialization.Queries.WildcardQuery

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.Utility

type WildcardQueryField with
    member x.ToJson() =
        match x with
        | PatternValue null ->
            Json.makeKeyValue "value" (Json.quoteString "")
        | PatternValue x ->
            Json.makeKeyValue "value" (Json.quoteString x)
        | Rewrite opt ->
            Json.makeKeyValue "rewrite" (Json.quoteString (opt.ToStringValue()))
        | Boost b ->
            Json.makeKeyValue "boost" (b.ToString())

let wildcardQueryToJson ((name, wildcardBody) : string * (WildcardQueryField list) ) =
    Json.makeObject [
        Json.makeKeyValue name (Json.makeObject [
            for field in wildcardBody ->
                field.ToJson()
        ])
    ]

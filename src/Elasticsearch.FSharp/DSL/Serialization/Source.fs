module internal Elasticsearch.FSharp.DSL.Serialization.Source

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.Utility

let private serializeList = List.map Json.quoteString >> Json.makeArray
type SourceBody with
    member x.ToJson() =
        match x with
        | Nothing ->
            "false"
        | Only value ->
            Json.quoteString value
        | List list ->
            serializeList list
        | Pattern (includes, excludes) ->
            Json.makeObject [
                Json.makeKeyValue "includes" (serializeList includes)
                Json.makeKeyValue "excludes" (serializeList excludes)
            ]
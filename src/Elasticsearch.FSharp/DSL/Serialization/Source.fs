module internal Elasticsearch.FSharp.DSL.Serialization.Source

open Elasticsearch.FSharp.DSL

let SerializeList l =
    "[" + (l |> List.map (fun field -> "\"" + field + "\"") |> String.concat ",") + "]"

let SourceBodyToJSON (body: SourceBody) : string =
    match body with
    | Nothing ->
        "false"
    | Only str ->
        "\"" + str + "\""
    | List l ->
        SerializeList l
    | Pattern (i, e) ->
        "{\"includes\":" + SerializeList i + ", \"excludes\":" + SerializeList e + "}"
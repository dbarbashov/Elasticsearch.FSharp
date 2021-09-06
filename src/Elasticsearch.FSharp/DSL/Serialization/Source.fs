module internal Elasticsearch.FSharp.DSL.Serialization.Source

open Elasticsearch.FSharp.DSL

let serializeList l =
    "[" + (l |> List.map (fun field -> "\"" + field + "\"") |> String.concat ",") + "]"

let sourceBodyToJson (body: SourceBody) : string =
    match body with
    | Nothing ->
        "false"
    | Only str ->
        "\"" + str + "\""
    | List l ->
        serializeList l
    | Pattern (i, e) ->
        "{\"includes\":" + serializeList i + ", \"excludes\":" + serializeList e + "}"
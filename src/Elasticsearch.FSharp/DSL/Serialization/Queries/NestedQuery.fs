module Elasticsearch.FSharp.DSL.Serialization.Queries.NestedQuery

open System
open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.Utility

let private scoreModeOptionToString (scoreMode: ScoreModeOption) : string =
    match scoreMode with
    | ScoreModeOption.ScoreModeAvg -> "avg"
    | ScoreModeOption.ScoreModeMax -> "max"
    | ScoreModeOption.ScoreModeMin -> "min"
    | ScoreModeOption.ScoreModeNone -> "none"
    | ScoreModeOption.ScoreModeSum -> "sum"

let nestedQueryToJson (queryBodySerializer: QueryBody -> string) (queryParams: NestedQueryField list) : string =
    let mutable pathOpt: string option = None
    let mutable queryOpt: QueryBody option = None
    let mutable scoreModeOpt: ScoreModeOption option = None
    let mutable ignoreUnmappedOpt: bool option = None

    for param in queryParams do
        match param with
        | NestedQueryField.Path p -> pathOpt <- Some p
        | NestedQueryField.QueryBody q -> queryOpt <- Some q
        | NestedQueryField.ScoreMode sm -> scoreModeOpt <- Some sm
        | NestedQueryField.IgnoreUnmapped iu -> ignoreUnmappedOpt <- Some iu

    let pathJson =
        match pathOpt with
        | Some p -> sprintf """"path":%s""" (Json.quoteString p)
        | None -> raise (ArgumentException("Nested query requires a 'path' field for serialization."))

    // For the inner query, we expect the serializer to produce the full query object string,
    // e.g., {"match_all":{}} or {"term":{"field":{"value":"val"}}}
    // The queryBodySerializer passed in should be `queryBodyToJson` from the main Query.fs serializer,
    // which already wraps the specific query in its own object like {"match": ...}
    let queryJson =
        match queryOpt with
        | Some q -> sprintf """"query":%s""" (queryBodySerializer q) 
        | None -> raise (ArgumentException("Nested query requires a 'query' field for serialization."))

    let optionalParts = ResizeArray()
    scoreModeOpt |> Option.iter (fun sm -> optionalParts.Add(sprintf """"score_mode":%s""" (Json.quoteString (scoreModeOptionToString sm))))
    ignoreUnmappedOpt |> Option.iter (fun iu -> optionalParts.Add(sprintf """"ignore_unmapped":%s""" (iu.ToString().ToLowerInvariant())))
    
    let allParts = [pathJson; queryJson] @ (List.ofSeq optionalParts)
    // This function produces the *content* of the "nested" query object, e.g. "path":"...", "query":{...}
    // The calling function (in DSL.Serialization.Query) will wrap this with {"nested": ... }
    String.concat "," allParts

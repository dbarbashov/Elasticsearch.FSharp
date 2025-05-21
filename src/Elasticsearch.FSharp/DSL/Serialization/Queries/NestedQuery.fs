module Elasticsearch.FSharp.DSL.Serialization.Queries.NestedQuery

open System
open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.Utility

// Private helper for ScoreModeOption to JSON string conversion
let private scoreModeOptionToString (scoreMode: ScoreModeOption) : string =
    match scoreMode with
    | ScoreModeOption.ScoreModeAvg -> "avg"
    | ScoreModeOption.ScoreModeMax -> "max"
    | ScoreModeOption.ScoreModeMin -> "min"
    | ScoreModeOption.ScoreModeNone -> "none"
    | ScoreModeOption.ScoreModeSum -> "sum"

// Private helper to convert a single NestedQueryField to its JSON key-value string part
let private nestedQueryFieldToJson (queryBodySerializer: QueryBody -> string) (field: NestedQueryField) : string =
    match field with
    | NestedQueryField.Path p ->
        Json.makeKeyValue "path" (Json.quoteString p)
    | NestedQueryField.QueryBody q ->
        Json.makeKeyValue "query" (queryBodySerializer q) // queryBodySerializer produces a string which is a valid JSON object
    | NestedQueryField.ScoreMode sm ->
        Json.makeKeyValue "score_mode" (Json.quoteString (scoreModeOptionToString sm))
    | NestedQueryField.IgnoreUnmapped iu ->
        Json.makeKeyValue "ignore_unmapped" (iu.ToString().ToLowerInvariant()) // JSON booleans are lowercase literals

// Main serialization function for the content of a "nested" query object
let nestedQueryToJson (queryBodySerializer: QueryBody -> string) (queryParams: NestedQueryField list) : string =
    // Validate that 'path' and 'query' fields are present, as they are required for a nested query.
    let hasPath = queryParams |> List.exists (function NestedQueryField.Path _ -> true | _ -> false)
    let hasQuery = queryParams |> List.exists (function NestedQueryField.QueryBody _ -> true | _ -> false)

    if not hasPath then
        raise (ArgumentException("Nested query requires a 'path' field. Please include 'Path \"your_path\"' in the query parameters."))
    if not hasQuery then
        raise (ArgumentException("Nested query requires a 'query' field. Please include 'QueryBody YourQueryDefinition' in the query parameters."))

    // Construct the JSON object from the list of query parameters.
    // Each parameter is converted to its JSON representation by nestedQueryFieldToJson.
    Json.makeObject [
        for param in queryParams ->
            nestedQueryFieldToJson queryBodySerializer param
    ]

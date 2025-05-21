# Nested Query

The `Nested` query allows you to query nested objects/documents as if they were separate documents. If an object is mapped as `nested` type, then this query is used to query its fields.

## F# DSL Example

```fsharp
open Elasticsearch.FSharp.DSL

let query =
    Search [
        Query (
            Nested [
                NestedQueryField.Path "user_comments" // Path to the nested objects
                NestedQueryField.QueryBody (          // Query to run on the nested documents
                    Bool [
                        Must [
                            Match ("user_comments.text", [MatchQuery "F# DSL"])
                            Range ("user_comments.stars", [Gte "4"])
                        ]
                    ]
                )
                NestedQueryField.ScoreMode ScoreModeOption.ScoreModeAvg // How scores from nested docs are aggregated
                NestedQueryField.IgnoreUnmapped true                   // If true, ignore if path is not a nested type or doesn't exist
            ]
        )
    ]
```

## JSON Output

```json
{
  "query": {
    "nested": {
      "path": "user_comments",
      "query": {
        "bool": {
          "must": [
            { "match": { "user_comments.text": { "query": "F# DSL" } } },
            { "range": { "user_comments.stars": { "gte": "4" } } }
          ]
        }
      },
      "score_mode": "avg",
      "ignore_unmapped": true
    }
  }
}
```

### Parameters for `NestedQueryField`

The `Nested` query takes a list of `NestedQueryField` elements:

-   `Path of string`: The path to the nested object field (e.g., "comments", "offer.variants"). This field must be mapped as `nested`.
-   `QueryBody of QueryBody`: The query to execute on the matching nested documents. Any valid `QueryBody` can be used here.
-   `ScoreMode of ScoreModeOption`: Defines how the scores of matching nested documents are aggregated into the parent document's score. The available options are:
    -   `ScoreModeAvg`: Use the average of all matching nested documents' scores.
    -   `ScoreModeMax`: Use the maximum score among all matching nested documents.
    -   `ScoreModeMin`: Use the minimum score among all matching nested documents.
    -   `ScoreModeNone`: Do not use the scores from the nested query. The parent document's score is not affected. (In F#, use `ScoreModeOption.ScoreModeNone`).
    -   `ScoreModeSum`: Use the sum of all matching nested documents' scores.
-   `IgnoreUnmapped of bool`: When set to `true`, if the `Path` does not point to a `nested` field type or the path does not exist in the mapping, the query will be ignored and will not throw an exception. Defaults to `false`.

### Example with `ScoreModeNone`

```fsharp
open Elasticsearch.FSharp.DSL

let query =
    Search [
        Query (
            Nested [
                NestedQueryField.Path "permissions"
                NestedQueryField.QueryBody (
                    Term ("permissions.role", [ExactValue "admin"])
                )
                NestedQueryField.ScoreMode ScoreModeOption.ScoreModeNone // Score of nested match doesn't affect parent
            ]
        )
    ]
```

JSON Output:
```json
{
  "query": {
    "nested": {
      "path": "permissions",
      "query": {
        "term": {
          "permissions.role": { "value": "admin" }
        }
      },
      "score_mode": "none"
    }
  }
}
```

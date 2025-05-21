# Multi Match Query

The `MultiMatch` query builds on the `match` query to allow multi-field queries.

## F# DSL Example

```fsharp
open Elasticsearch.FSharp.DSL

let query =
    Search [
        Query (
            MultiMatch [
                MultiMatchQueryField.MultiMatchQuery "quick brown fox"
                MultiMatchQueryField.Fields ["title"; "body^2"; "*.subject"] // Fields can have boosts and wildcards
                MultiMatchQueryField.QueryType "best_fields" // e.g., "best_fields", "most_fields", "cross_fields", "phrase", "phrase_prefix"
                MultiMatchQueryField.TieBreaker 0.3
                MultiMatchQueryField.MaxExpansions 10 // For phrase_prefix type
                MultiMatchQueryField.Slop 2           // For phrase type
            ]
        )
    ]
```

## JSON Output

```json
{
  "query": {
    "multi_match": {
      "query": "quick brown fox",
      "fields": ["title", "body^2", "*.subject"],
      "type": "best_fields",
      "tie_breaker": 0.3,
      "max_expansions": 10,
      "slop": 2
    }
  }
}
```

### Parameters for `MultiMatchQueryField`

- `MultiMatchQuery of string`: The query string.
- `Fields of string list`: The list of fields to search. Wildcards and per-field boosts (e.g., `"my_field^3"`) are supported.
- `QueryType of string`: The type of `multi_match` query (e.g., `best_fields`, `most_fields`, `cross_fields`, `phrase`, `phrase_prefix`, `bool_prefix`).
- `TieBreaker of float`: Used with `best_fields` type. A value between 0 and 1.
- `MaxExpansions of int`: Used with `phrase_prefix` type. Controls how many terms the last term in the phrase can expand to.
- `Slop of int`: Used with `phrase` and `phrase_prefix` types. The slop factor for phrase matching.
- `MultiMatchRaw of Key: string * Value: string`: Allows adding arbitrary key-value pairs to the multi_match query body. The `Value` should be a valid JSON formatted string.

### Example with Raw Parameter

```fsharp
let query =
    Search [
        Query (
            MultiMatch [
                MultiMatchQuery "search text"
                MultiMatchQueryField.MultiMatchRaw ("custom_param", "\"custom_value\"") // Value must be a JSON-formatted string
                MultiMatchQueryField.MultiMatchRaw ("custom_numeric", "123.45")      // Value can be a JSON-formatted number
            ]
        )
    ]
```

JSON Output:
```json
{
  "query": {
    "multi_match": {
      "query": "search text",
      "custom_param": "custom_value",
      "custom_numeric": 123.45
    }
  }
}
```

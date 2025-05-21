# Match None Query

The `MatchNone` query matches no documents.

## F# DSL Example

```fsharp
open Elasticsearch.FSharp.DSL

let query =
    Search [
        Query MatchNone
    ]
```

## JSON Output

```json
{
  "query": {
    "match_none": {}
  }
}
```

# Match All Query

The `MatchAll` query matches all documents.

## F# DSL Example

```fsharp
open Elasticsearch.FSharp.DSL

let query =
    Search [
        Query MatchAll
    ]
```

## JSON Output

```json
{
  "query": {
    "match_all": {}
  }
}
```

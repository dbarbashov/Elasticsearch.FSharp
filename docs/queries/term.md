# Term Query

The `Term` query finds documents that contain the exact term specified in the inverted index. It is not analyzed.

## F# DSL Example

```fsharp
open Elasticsearch.FSharp.DSL

let query =
    Search [
        Query (
            Term ("user.id", [ExactValue "kimchy"])
        )
    ]
```

## JSON Output

```json
{
  "query": {
    "term": {
      "user.id": {
        "value": "kimchy"
      }
    }
  }
}
```

### Parameters for `TermQueryField`

- `ExactValue of string`: The exact term to search for.

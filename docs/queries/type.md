# Type Query

The `Type` query filters documents of a specific type.

**Important Note:** The `_type` field was deprecated in Elasticsearch 6.0, and completely removed in Elasticsearch 7.0 for indices created in 7.0+. For indices created in 6.x that had multiple types, the `_type` field still exists. In Elasticsearch 8.0+, types are entirely removed. This query is primarily for interacting with older indices or specific legacy setups.

## F# DSL Example

```fsharp
open Elasticsearch.FSharp.DSL

let query =
    Search [
        Query (TypeEquals "my_document_type")
    ]
```

## JSON Output

```json
{
  "query": {
    "type": {
      "value": "my_document_type"
    }
  }
}
```

# Exists Query

The `Exists` query returns documents that have a value for a specified field. This includes documents where the field has an explicit `null` value or an empty array.

## F# DSL Example

```fsharp
open Elasticsearch.FSharp.DSL

let query =
    Search [
        Query (Exists "user.id")
    ]
```

## JSON Output

```json
{
  "query": {
    "exists": {
      "field": "user.id"
    }
  }
}
```

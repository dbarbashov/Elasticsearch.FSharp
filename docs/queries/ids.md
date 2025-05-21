# Ids Query

The `Ids` query filters documents that only have the specified IDs.

## F# DSL Example

```fsharp
open Elasticsearch.FSharp.DSL

let query =
    Search [
        Query (Ids ["foo"; "bar"])
    ]
```

## JSON Output

```json
{
  "query": {
    "ids": {
      "values": ["foo", "bar"]
    }
  }
}
```

# Raw Query

The `Raw` query allows you to embed a raw JSON string as part of your query. This is useful for query types not directly supported by the DSL or for complex, custom JSON structures.

## F# DSL Example

```fsharp
open Elasticsearch.FSharp.DSL

// The string must be a valid JSON object representing the query part.
let rawJsonQuery = """{ "term": { "user.id": { "value": "specific_user", "boost": 2.0 } } }"""

let query =
    Search [
        Query (QueryBody.Raw rawJsonQuery)
    ]
```

## JSON Output

The content of `rawJsonQuery` will be directly embedded within the `"query": { ... }` structure:

```json
{
  "query": {
    "term": {
      "user.id": {
        "value": "specific_user",
        "boost": 2.0
      }
    }
  }
}
```

**Important:** The string provided to `QueryBody.Raw` must itself be a valid JSON object that can fit into the `query` clause. The DSL does not validate this JSON string; it embeds it as is.

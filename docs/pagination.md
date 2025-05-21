# Pagination: From and Size

You can control pagination of search results using the `From` and `Size` elements in your `Search` request.

- `From of int`: The starting document offset (0-indexed). Defaults to 0.
- `Size of int`: The number of hits to return. Defaults to 10.

## F# DSL Example

```fsharp
open Elasticsearch.FSharp.DSL

let query =
    Search [
        From 300
        Size 100
        Query MatchAll
    ]
```

## JSON Output

```json
{
  "from": 300,
  "size": 100,
  "query": {
    "match_all": {}
  }
}
```
Be mindful of deep pagination (large `from` values) as it can be resource-intensive. For deep pagination, consider using `search_after` or scroll APIs, which are not directly modeled in this DSL but can be constructed using `Raw` elements if needed.

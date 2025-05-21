# Terms Query

The `Terms` query filters documents that have fields that match any of the provided terms (not analyzed).

## F# DSL Example (Value List)

```fsharp
open Elasticsearch.FSharp.DSL

let query =
    Search [
        Query (
            Terms ("tags", [ValueList ["blue"; "green"]])
        )
    ]
```

## JSON Output (Value List)

```json
{
  "query": {
    "terms": {
      "tags": ["blue", "green"]
    }
  }
}
```

## F# DSL Example (Terms Lookup)

You can also specify terms using a terms lookup mechanism, fetching terms from another document.

```fsharp
open Elasticsearch.FSharp.DSL

let query =
    Search [
        Query (
            Terms ("user.id", [FromIndex ("users_index", "user_type", "document_id", "followers_field")])
        )
    ]
```

## JSON Output (Terms Lookup)

```json
{
  "query": {
    "terms": {
      "user.id": {
        "index": "users_index",
        "type": "user_type",
        "id": "document_id",
        "path": "followers_field"
      }
    }
  }
}
```

### Parameters for `TermsQueryField`

- `ValueList of string list`: A list of exact terms to match.
- `FromIndex of string * string * string * string`: Specifies lookup parameters: `index`, `type` (Elasticsearch document type), `id`, and `path` (field in the lookup document).

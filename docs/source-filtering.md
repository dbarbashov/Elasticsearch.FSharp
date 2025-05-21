# Source Filtering

Source filtering allows you to control which parts of the original document (`_source` field) are returned in the search results. Add a `Source_` element (of type `SearchBody.Source_`) to your `Search` request.

## F# DSL Examples

### Disable Source Retrieval

To disable retrieval of the `_source` field entirely:

```fsharp
open Elasticsearch.FSharp.DSL

let query =
    Search [
        Source_ Nothing
        Query MatchAll
    ]
```

JSON Output:
```json
{
  "_source": false,
  "query": { "match_all": {} }
}
```

### Retrieve Only a Specific Field

To retrieve only a single field (this form might be limited or act as an include depending on ES version):

```fsharp
let query =
    Search [
        Source_ (Only "title")
        Query MatchAll
    ]
```

JSON Output:
```json
{
  "_source": "title",
  "query": { "match_all": {} }
}
```
*Note: More commonly, for specific fields, you'd use the list or pattern approach.*

### Retrieve a List of Fields

To retrieve a specific list of fields:

```fsharp
let query =
    Search [
        Source_ (List ["title"; "author.name"; "date"])
        Query MatchAll
    ]
```

JSON Output:
```json
{
  "_source": ["title", "author.name", "date"],
  "query": { "match_all": {} }
}
```

### Use Include/Exclude Patterns

To use include and exclude patterns (wildcards are supported):

```fsharp
let query =
    Search [
        Source_ (Pattern (Includes = ["obj.*", "user.id"], Excludes = ["*.raw", "obj.secret"]))
        Query MatchAll
    ]
```

JSON Output:
```json
{
  "_source": {
    "includes": ["obj.*", "user.id"],
    "excludes": ["*.raw", "obj.secret"]
  },
  "query": { "match_all": {} }
}
```

## `SourceBody` Options

- `Nothing`: Corresponds to `_source: false`. The `_source` field is not returned.
- `Only of string`: Specifies a single field name as a string. Elasticsearch interprets this as including only this field.
- `List of string list`: Specifies a list of fields to retrieve. Equivalent to `includes` with an empty `excludes`.
- `Pattern of Includes: string list * Excludes: string list`: Specifies `includes` and `excludes` patterns for fine-grained control.

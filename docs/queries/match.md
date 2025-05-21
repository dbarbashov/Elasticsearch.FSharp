# Match Query

The `Match` query is a standard query for performing full-text search, including options for fuzzy matching.

## F# DSL Example

```fsharp
open Elasticsearch.FSharp.DSL

let query =
    Search [
        Query (
            Match ("message", [
                MatchQueryField.MatchQuery "this is a test"
                MatchQueryField.Operator "and"
                MatchQueryField.ZeroTermsQuery "all"
                MatchQueryField.CutoffFrequency 0.001
            ])
        )
    ]
```

## JSON Output

```json
{
  "query": {
    "match": {
      "message": {
        "query": "this is a test",
        "operator": "and",
        "zero_terms_query": "all",
        "cutoff_frequency": 0.001
      }
    }
  }
}
```

### Parameters for `MatchQueryField`

- `MatchQuery of string`: The text to search for.
- `Operator of string`: The boolean operator for the query terms (e.g., "or", "and").
- `ZeroTermsQuery of string`: What to do if the analyzer removes all terms (e.g., "none", "all").
    *Note: The current serialization for `ZeroTermsQuery` in `src/Elasticsearch.FSharp/DSL/Serialization/Queries/MatchQuery.fs` uses `x.ToString()`, which for a string like "all" results in `all` (unquoted) in the JSON. This is the expected format by Elasticsearch for this specific parameter.*
- `CutoffFrequency of double`: A value between 0 and 1 (exclusive) that acts as a threshold for term relevance.

# Wildcard Query

The `Wildcard` query matches documents that have fields matching a wildcard expression (not analyzed).

## F# DSL Example

```fsharp
open Elasticsearch.FSharp.DSL

let query =
    Search [
        Query (Wildcard ("user.id", [
            PatternValue "ki*y"
            WildcardQueryField.Boost 1.2
            WildcardQueryField.Rewrite RewriteOption.ConstantScore
        ]))
    ]
```

## JSON Output

```json
{
  "query": {
    "wildcard": {
      "user.id": {
        "value": "ki*y",
        "boost": 1.2,
        "rewrite": "constant_score"
      }
    }
  }
}
```

### Parameters for `WildcardQueryField`

- `PatternValue of string`: The wildcard pattern.
- `WildcardQueryField.Boost of float`: Sets the boost value for the query.
- `WildcardQueryField.Rewrite of RewriteOption`: Determines how the query is rewritten. `RewriteOption` includes values like:
    - `ConstantScoreBlended`
    - `ConstantScore`
    - `ConstantScoreBoolean`
    - `ScoringBoolean`
    - `TopTermsBlendedFreqs of int`
    - `TopTermsBoost of int`
    - `TopTerms of int`

### Example with `TopTerms N` Rewrite

```fsharp
let query =
    Search [
        Query (
            Wildcard ("user.id", [
                PatternValue "ki*y"
                WildcardQueryField.Rewrite (RewriteOption.TopTerms 10)
            ])
        )
    ]
```

JSON Output:
```json
{
  "query": {
    "wildcard": {
      "user.id": {
        "value": "ki*y",
        "rewrite": "top_terms_10"
      }
    }
  }
}
```

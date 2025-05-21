# Match Phrase Prefix Query

The `MatchPhrasePrefix` query is similar to the `match_phrase` query, but it allows for a prefix match on the last term in the phrase.

## F# DSL Example

```fsharp
open Elasticsearch.FSharp.DSL

let query =
    Search [
        Query (MatchPhrasePrefix ("message", [
            MatchPhrasePrefixQueryField.MatchQuery "quick brown f"
            MatchPhrasePrefixQueryField.MaxExpansions 10
            MatchPhrasePrefixQueryField.Boost 0.8
            MatchPhrasePrefixQueryField.Slop 2
            MatchPhrasePrefixQueryField.Analyzer "standard"
        ]))
    ]
```

## JSON Output

```json
{
  "query": {
    "match_phrase_prefix": {
      "message": {
        "query": "quick brown f",
        "max_expansions": 10,
        "boost": 0.8,
        "slop": 2,
        "analyzer": "standard"
      }
    }
  }
}
```

### Parameters for `MatchPhrasePrefixQueryField`

- `MatchQuery of string`: The text to search for.
- `MaxExpansions of int`: Controls how many terms the last term in the phrase can expand to.
- `Slop of int`: The slop factor for phrase matching.
- `Analyzer of string`: The analyzer to use.
- `Boost of float`: Sets the boost value for the query.

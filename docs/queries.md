# Match All Query

Query:
```f#
let query =
    Search [
        Query MatchAll
    ]
```
JSON:
```json
{
  "query": {
    "match_all": {}
  }
}
```

# Match None Query
Query:
```f#
let query =
    Search [
        Query MatchNone
    ]
```
JSON:
```json
{
  "query": {
    "match_none": {}
  }
}
```
# Ids Query 
Query:
```f#
let query = 
    Search [
        Query (Ids ["foo"; "bar"])
    ]
```

JSON: 
```json
{
  "query": { 
    "ids": {
      "values": ["foo","bar"]
    }
  }
}
```

# Wildcard Query
Query:
```f#
let query =
    Search [
        Query (Wildcard ("user.id", [
            PatternValue "ki*y"
            WildcardQueryField.Boost 1.2
            WildcardQueryField.Rewrite RewriteOption.ConstantScore
        ]))
    ]
```
JSON:
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

# Match Phrase Prefix Query
Query:
```f#
let query =
    Search [
        Query (MatchPhrasePrefix ("message", [
            MatchPhrasePrefixQueryField.MatchQuery "quick brown f"
            MatchPhrasePrefixQueryField.MaxExpansions 10
            MatchPhrasePrefixQueryField.Boost 0.8
        ]))
    ]
```
JSON:
```json
{
  "query": {
    "match_phrase_prefix": {
      "message": {
        "query": "quick brown f",
        "max_expansions": 10,
        "boost": 0.8
      }
    }
  }
}
```

# Nested Query
Query:
```f#
let query =
    Search [
        Query (
            Nested [
                NestedQueryField.Path "my_nested_path"
                NestedQueryField.QueryBody (
                    Match ("my_nested_path.field", [MatchQuery "value"])
                )
            ]
        )
    ]
```
JSON:
```json
{
  "query": {
    "nested": {
      "path": "my_nested_path",
      "query": {
        "match": { "my_nested_path.field": { "query": "value" } }
      }
    }
  }
}
```

# Bool Query

The `Bool` query matches documents matching boolean combinations of other queries.

## F# DSL Example

```fsharp
open Elasticsearch.FSharp.DSL

let query =
    Search [
        Query (
            Bool [
                Must [ Match ("title", [MatchQuery "elasticsearch"]) ]
                Filter [ Term ("status", [ExactValue "published"]) ]
                MustNot [ Term ("tags", [ExactValue "archived"]) ]
                Should [ Match ("content", [MatchQuery "relevant"]) ]
                MinimumShouldMatch "1"
            ]
        )
    ]
```

## JSON Output

```json
{
  "query": {
    "bool": {
      "must": [
        { "match": { "title": { "query": "elasticsearch" } } }
      ],
      "filter": [
        { "term": { "status": { "value": "published" } } }
      ],
      "must_not": [
        { "term": { "tags": { "value": "archived" } } }
      ],
      "should": [
        { "match": { "content": { "query": "relevant" } } }
      ],
      "minimum_should_match": "1"
    }
  }
}
```

### Clauses

The `Bool` query takes a list of `BoolQuery` elements:
- `Must of QueryBody list`: The clause (query) must appear in matching documents.
- `Filter of QueryBody list`: The clause (query) must appear in matching documents. However, unlike `must`, the score of the query will be ignored (it functions in a filter context).
- `Should of QueryBody list`: The clause (query) should appear in matching documents. In a boolean query with no `must` or `filter` clauses, one or more `should` clauses must match a document.
- `MustNot of QueryBody list`: The clause (query) must not appear in the matching documents.
- `MinimumShouldMatch of string`: Specifies the number or percentage of `should` clauses that must match.

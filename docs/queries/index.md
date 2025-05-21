# Query Types

This section provides details on the various query types supported by the Elasticsearch.FSharp DSL. Each query type is a variant of the `QueryBody` discriminated union, typically used within a `Search [ Query (...) ]` construct.

## Available Query Types

- [Match All Query](./match-all.md)
- [Match None Query](./match-none.md)
- [Ids Query](./ids.md)
- [Bool Query](./bool.md)
- [Match Query](./match.md)
- [Term Query](./term.md)
- [Terms Query](./terms.md)
- [Range Query](./range.md)
- [Wildcard Query](./wildcard.md)
- [Script Query](./script.md)
- [Multi Match Query](./multi-match.md)
- [Match Phrase Prefix Query](./match-phrase-prefix.md)
- [Nested Query](./nested.md)
- [Exists Query](./exists.md)
- [Type Query](./type.md)
- [Raw Query](./raw.md)

Select a query type from the list above to see its specific F# DSL usage and the corresponding JSON output.

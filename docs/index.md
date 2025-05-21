# Welcome to Elasticsearch.FSharp DSL Documentation

This library provides an F# Domain Specific Language (DSL) for constructing Elasticsearch queries.

## Getting Started

This documentation provides examples of how to use the DSL to construct various Elasticsearch requests. Ensure you have the `Elasticsearch.FSharp` package installed in your project.

## Documentation Sections

- [Search DSL Overview](./search-dsl.md): Learn about the core components of a search request.
- [Queries](./queries/index.md): Detailed information on various query types.
- [Aggregations](./aggregations.md): How to use aggregations.
- [Sorting](./sort.md): Sorting your search results.
- [Pagination](./pagination.md): Controlling 'from' and 'size' of results.
- [Script Fields](./script-fields.md): Using script fields in your search.
- [Source Filtering](./source-filtering.md): Controlling which parts of the source document are returned.
- [Mapping Generation](./mapping.md): Generating Elasticsearch mappings from F# types.

## Examples

Throughout this documentation, you will find F# code examples and the corresponding JSON output they generate for Elasticsearch. All F# examples assume you have the relevant namespaces open, primarily:

```fsharp
open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization // For toJson function if used directly
```

# Elasticsearch.FSharp

Compose type-checked ElasticSearch queries with the power of F# language.

![Nuget Version](https://img.shields.io/nuget/v/dbarbashov.Elasticsearch.FSharp)
![Github Actions Badge](https://github.com/dbarbashov/Elasticsearch.FSharp/actions/workflows/build-and-test.yml/badge.svg)

## Example

F# query:
```f#
let query = 
    Search [
        Sort [
            "timestamp", [Order SortOrder.Desc]
        ]
        Query MatchAll
    ]
```
Would result in following JSON:
```json
{
  "sort": [
    { "timestamp": { "order": "asc" } }
  ],
  "query": {
    "match_all": {}
  }
}
```

## Features
* Compose strongly-typed F#-first ElasticSearch queries
* Generate index mappings by assigning attributes to your types

## Supported Elastic versions
* ElasticSearch 6

## Roadmap
### Queries
* [x] Size
* [x] From
* [x] Sort
  * [x] Sort order 
  * [x] Sort mode
* [ ] Query
  * [x] Match all query
  * [x] Match none query
  * [x] Ids query
  * [x] Bool query
    * [x] Must
    * [x] Must not
    * [x] Should
    * [x] Filter
    * [x] MinimumShouldMatch
  * [x] Match query
  * [x] Term query 
  * [x] Terms query 
  * [x] Range query
  * [x] Script query
  * [x] MultiMatch query
  * [x] MatchPhrasePrefix query
  * [x] Exists query
  * [x] Type query
  * [x] Wildcard query
    * [x] `rewrite` and `boost` parameters

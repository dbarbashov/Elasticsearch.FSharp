# Sorting Results

You can sort your search results by adding a `Sort` element to the `Search` list. The `Sort` element takes a list of sort criteria, where each criterion is a tuple of field name and a list of `SortField` options.

## F# DSL Example

```fsharp
open Elasticsearch.FSharp.DSL

let query =
    Search [
        Sort [
            "timestamp", [Order SortOrder.Desc; Mode SortMode.Avg]
            "name.keyword", [Order SortOrder.Asc] // Example of sorting on another field
            "_score", [Order SortOrder.Desc]      // Sorting by score
        ]
        Query MatchAll
    ]
```

## JSON Output

```json
{
  "sort": [
    { "timestamp": { "order": "desc", "mode": "avg" } },
    { "name.keyword": { "order": "asc" } },
    { "_score": { "order": "desc" } }
  ],
  "query": {
    "match_all": {}
  }
}
```

### Parameters for `SortField`

Each entry in the `Sort` list is a `SortBody` which is `string * (SortField list)`.
- `Order of SortOrder`: Specifies the sort order.
    - `SortOrder.Asc` (ascending)
    - `SortOrder.Desc` (descending)
- `Mode of SortMode`: Applicable for sorting on array or multi-valued fields. Determines which value is selected for sorting.
    - `SortMode.Min`: Pick the lowest value.
    - `SortMode.Max`: Pick the highest value.
    - `SortMode.Sum`: Use the sum of values as the sort value.
    - `SortMode.Avg`: Use the average of values as the sort value.
    - `SortMode.Median`: Use the median of values as the sort value.

If `Mode` is not specified for a multi-valued field, Elasticsearch will choose a default (usually `min` for ascending order and `max` for descending order, depending on the field type and context).

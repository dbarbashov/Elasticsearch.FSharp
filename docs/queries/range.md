# Range Query

The `Range` query matches documents with fields that have terms within a certain range.

## F# DSL Example

```fsharp
open Elasticsearch.FSharp.DSL

let query =
    Search [
        Query (
            Range ("timestamp", [
                Gte "2024-01-01T00:00:00Z"
                Lte "now/d" // Date math is supported
                RangeTimeZone "+01:00"
            ])
        )
    ]
```

## JSON Output

```json
{
  "query": {
    "range": {
      "timestamp": {
        "gte": "2024-01-01T00:00:00Z",
        "lte": "now/d",
        "time_zone": "+01:00"
      }
    }
  }
}
```

### Parameters for `RangeQueryField`

- `Gte of string`: Greater than or equal to.
- `Gt of string`: Greater than.
- `Lte of string`: Less than or equal to.
- `Lt of string`: Less than.
- `RangeTimeZone of string`: Time zone for date ranges (e.g., "+01:00" or "Europe/Amsterdam").

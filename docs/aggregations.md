# Aggregations

Aggregations allow you to group and extract statistics from your data. Add an `Aggs` element to your `Search` request. The `Aggs` element takes a list of `AggsFieldsBody`.

## F# DSL Example: Simple Average Aggregation

```fsharp
open Elasticsearch.FSharp.DSL

let query =
    Search [
        Aggs [
            NamedAgg (
                "average_price", // Name of the aggregation
                Avg [ AggField "price" ] // Aggregation type and parameters
            )
        ]
        Query MatchAll
    ]
```

## JSON Output

```json
{
  "query": {
    "match_all": {}
  },
  "aggs": {
    "average_price": {
      "avg": {
        "field": "price"
      }
    }
  }
}
```

## Aggregation Structures (`AggsFieldsBody`)

- `NamedAgg of string * AggBody`: A simple named aggregation.
- `NamedComplexAgg of string * AggBody * AggsFieldsBody list`: A named aggregation that can have sub-aggregations.
- `FilterAgg of string * QueryBody * AggBody`: An aggregation that operates on a filtered subset of documents.
- `FilterComplexAgg of string * QueryBody * AggBody * AggsFieldsBody list`: A filtered aggregation that can have sub-aggregations.

## Aggregation Types (`AggBody`)

- `Avg of AggParam list`: Calculates the average of a numeric field.
- `WeightedAvg of AggParam list`: Calculates a weighted average.
- `Max of AggParam list`: Finds the maximum value.
- `Min of AggParam list`: Finds the minimum value.
- `Sum of AggParam list`: Calculates the sum.
- `Stats of AggParam list`: Returns multiple statistics (min, max, sum, count, avg).
- `AggTerms of AggParam list`: A multi-bucket aggregation that creates buckets based on field values.
- `AggDateHistogram of AggParam list`: A multi-bucket aggregation that builds buckets based on time intervals.
- `ValueCount of AggParam list`: Counts the number of documents that have a value for a field.

## Aggregation Parameters (`AggParam`)

- `AggField of string`: The field to aggregate on.
- `AggScript of ScriptField list`: Use a script to generate values for aggregation.
- `AggValue of string`: A specific value (usage depends on aggregation type).
- `AggWeight of AggWeightConfig`: For weighted average, specifies weight field or value.
  - `WeightField of string`
  - `WeightValueField of string`
  - `Weight of string` (numeric weight as string)
- `AggInterval of string`: For date histograms (e.g., "month", "1d", "1h").
- `AggFormat of string`: Format for date histograms or other formatted values (e.g., "yyyy-MM-dd").
- `AggSize of int`: For terms aggregations, the number of buckets to return.

## Example: Weighted Average Aggregation

```fsharp
let query =
    Search [
        Aggs [
            NamedAgg (
                "weighted_score_avg",
                WeightedAvg [
                    AggWeight (WeightValueField "score") // Field containing the value
                    AggWeight (WeightField "weight")    // Field containing the weight
                ]
            )
        ]
    ]
```

JSON Output:
```json
{
  "aggs": {
    "weighted_score_avg": {
      "weighted_avg": {
        "value": { "field": "score" },
        "weight": { "field": "weight" }
      }
    }
  }
}
```

## Example: Value Count Aggregation

```fsharp
let query =
    Search [
        Aggs [
            NamedAgg (
                "type_count",
                ValueCount [ AggField "document_type.keyword" ]
            )
        ]
    ]
```

JSON Output:
```json
{
  "aggs": {
    "type_count": {
      "value_count": {
        "field": "document_type.keyword"
      }
    }
  }
}
```

## Complex (Nested) Aggregations

Aggregations can be nested using `NamedComplexAgg` or `FilterComplexAgg`.

```fsharp
open Elasticsearch.FSharp.DSL

let query =
    Search [
        Aggs [
            NamedComplexAgg (
                "products_by_category", // Outer aggregation name
                AggTerms [ AggField "category.keyword"; AggSize 10 ], // Outer aggregation: terms on category
                [ // Inner (sub) aggregations
                    NamedAgg (
                        "average_price_in_category", // Inner agg name
                        Avg [ AggField "price" ]       // Inner agg: average price
                    ),
                    FilterComplexAgg (
                        "sales_for_popular_items_in_category", // Inner filtered agg name
                        Match ("popularity", [MatchQueryField.MatchQuery "high"]), // Filter for this inner agg
                        Sum [ AggField "sales_count" ], // Aggregation (sum of sales)
                        [ // Further nested aggregation within the filtered one
                            NamedAgg(
                                "avg_rating_for_popular_sales",
                                Avg [ AggField "rating"]
                            )
                        ]
                    )
                ]
            )
        ]
        Query MatchAll
    ]
```

JSON Output (structure based on `Complex aggs serializes correctly` test):
```json
{
  "query": { "match_all": {} },
  "aggs": {
    "products_by_category": {
      "terms": { "field": "category.keyword", "size": 10 },
      "aggs": {
        "average_price_in_category": {
          "avg": { "field": "price" }
        },
        "sales_for_popular_items_in_category": {
          "filter": { "match": { "popularity": { "query": "high" } } },
          "aggs": {
            "sales_for_popular_items_in_category": { 
               "sum": { "field": "sales_count" },
               "aggs": {
                  "avg_rating_for_popular_sales": {
                     "avg": { "field": "rating" }
                  }
               }
            }
          }
        }
      }
    }
  }
}
```
*Note: The name of the aggregation in the `FilterComplexAgg`'s "aggs" block is repeated in the JSON output by the current serializer. This is reflected in the example above, matching the behavior observed in tests.*

# Search Examples

## Query
All queries are done by specifying Query value in list of Search values:
```f#
let query = 
    Search [
        Query MatchAll
    ]
```
Query above would produce following JSON:
```json
{
  "query": {
    "match_all": {}
  }
}
```

## Sort 
It is possible to sort results by adding `Sort` value to the Search list:
```f#
let query = 
    Search [
        Sort [
            "timestamp", [Order SortOrder.Desc]
        ]
        Query MatchAll
    ]
```

The example would produce following JSON:
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

## From and Size
It is possible to specify from and size values
```f#
let query = 
    Search [
        From 300
        Size 100
        Query MatchAll
    ]
```
Resulting JSON: 
```json
{
  "from": 300,
  "size": 100,
  "query": {
    "match_all": {}
  }
}
```

## Track Total Hits
You can control if Elasticsearch should track the total number of hits accurately.
By default, for performance reasons, Elasticsearch might stop counting hits after a certain threshold (e.g., 10,000) and `hits.total.value` will be that threshold, with `hits.total.relation` being "gte" (greater than or equal to).
Set `TrackTotalHits true` to get an accurate count. Set to `false` to allow early termination of counting.

```f#
let query = 
    Search [
        TrackTotalHits true // Get an accurate total hit count
        Query (Term ("user.id", [ExactValue "kimchy"]))
    ]
```
Resulting JSON:
```json
{
  "track_total_hits": true,
  "query": {
    "term": {
      "user.id": { "value": "kimchy" }
    }
  }
}
```

```f#
let queryNoTrack =
    Search [
        TrackTotalHits false // Allow Elasticsearch to optimize by not counting all hits
        Query MatchAll
    ]
```
Resulting JSON:
```json
{
  "track_total_hits": false,
  "query": {
    "match_all": {}
  }
}
```

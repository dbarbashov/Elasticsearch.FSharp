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
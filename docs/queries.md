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
# Script Fields

Script fields allow you to evaluate a script on every hit and return the result as a field. This is useful for custom calculations or formatting data directly in the search response.

Add a `ScriptFields` element (of type `SearchBody.ScriptFields`) to your `Search` request.

## F# DSL Example

```fsharp
open Elasticsearch.FSharp.DSL

let query =
    Search [
        Query MatchAll
        ScriptFields [ // This is SearchBody.ScriptFields
            "my_calculated_field", [ // Name of the script field
                ScriptField.Lang "painless"
                ScriptField.Source "doc['price'].value * params.multiplier"
                ScriptField.Params [ "multiplier", "1.2" ]
            ]
            "document_id_field", [
                ScriptField.Source "doc['_id'].value" // Accessing metadata fields
            ]
        ]
    ]
```

## JSON Output

```json
{
  "query": {
    "match_all": {}
  },
  "script_fields": {
    "my_calculated_field": {
      "script": {
        "lang": "painless",
        "source": "doc['price'].value * params.multiplier",
        "params": {
          "multiplier": "1.2"
        }
      }
    },
    "document_id_field": {
      "script": {
        "source": "doc['_id'].value"
      }
    }
  }
}
```

### Parameters for `ScriptField` list

Each entry in `ScriptFields` is a `ScriptFieldsBody`, which is a tuple of `string * ScriptField list`. The `ScriptField list` can contain:
- `ScriptField.Source of string`: The script source code.
- `ScriptField.Lang of string`: The scripting language (e.g., "painless"). Defaults may apply if not specified.
- `ScriptField.Params of (string * string) list`: Parameters to pass to the script.
- `ScriptField.ScriptId of string`: The ID of a stored script to use (instead of `Source` and `Lang`).

## Top-Level `SearchBody.Script`

There is also a `SearchBody.Script` option. This is different from `ScriptFields` and `QueryBody.Script`. It's used for operations like scripted updates or other search contexts where a script is applied at the top level of the request, not for returning computed fields per document or as a query condition.

```fsharp
let query =
    Search [
        Query MatchAll // Or other query elements
        SearchBody.Script [ // Note: SearchBody.Script, not ScriptFields or QueryBody.Script
            Script.Source "ctx._source.counter += params.count" // Example for an update script
            Script.Lang "painless"
            Script.Params [ "count", "1" ]
        ]
    ]
```

JSON Output:
```json
{
  "query": { "match_all": {} },
  "script": {
    "source": "ctx._source.counter += params.count",
    "lang": "painless",
    "params": { "count": "1" }
  }
}
```
This `SearchBody.Script` is serialized directly under the root of the search request body.

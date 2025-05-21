# Script Query

The `Script` query allows you to use a script to evaluate if a document matches. This is part of the `QueryBody` options.

## F# DSL Example

```fsharp
open Elasticsearch.FSharp.DSL

let query =
    Search [
        Query (
            QueryBody.Script [ // Note: QueryBody.Script
                Script.Source "doc['num1'].value > params.param1"
                Script.Lang "painless"
                Script.Params [ "param1", "5" ]
            ]
        )
    ]
```

## JSON Output

```json
{
  "query": {
    "script": {
      "script": {
        "source": "doc['num1'].value > params.param1",
        "lang": "painless",
        "params": {
          "param1": "5"
        }
      }
    }
  }
}
```

### Parameters for `ScriptBody` (list of `Script` elements)

The `QueryBody.Script` takes a `ScriptBody`, which is a list of `Script` elements:
- `Script.Source of string`: The script source code.
- `Script.Lang of string`: The scripting language (e.g., "painless").
- `Script.Params of (string * string) list`: Parameters to pass to the script.

*Note: This is different from `SearchBody.ScriptFields` (for retrieving scripted values) and `SearchBody.Script` (for top-level search scripts like updates).*

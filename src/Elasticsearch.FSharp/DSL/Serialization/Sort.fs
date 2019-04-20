module internal Elasticsearch.FSharp.DSL.Serialization.Sort

open Elasticsearch.FSharp.DSL

let SortModeToJson (sortMode:SortMode) = 
    match sortMode with 
    | SortMode.Min -> "\"min\""
    | SortMode.Max -> "\"max\""
    | SortMode.Sum -> "\"sum\""
    | SortMode.Avg -> "\"avg\""
    | SortMode.Median -> "\"median\""
    
let SortOrderToJson sortOrder =
    match sortOrder with
    | SortOrder.Asc -> "\"asc\""
    | SortOrder.Desc -> "\"desc\""
    
let internal SortFieldListToJson (sortFieldList : SortField list ) = 
    "{" + 
        ([
            for field in sortFieldList ->  
                match field with 
                | Order orderVal -> 
                    "\"order\":" + (SortOrderToJson orderVal)
                | Mode modeVal -> 
                    "\"mode\":" + (SortModeToJson modeVal)
        ] |> String.concat ",")
    + "}"
    
let internal SortBodyListToJson (sortBody:(string * (SortField list)) list) = 
    "[" +
        ([
            for (name, fields) in sortBody -> 
                let body = SortFieldListToJson fields 
                "{\"" + name + "\":" + body + "}"
        ] |> String.concat ",")
    + "]"
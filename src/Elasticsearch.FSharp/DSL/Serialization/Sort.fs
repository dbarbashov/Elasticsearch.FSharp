module internal Elasticsearch.FSharp.DSL.Serialization.Sort

open Elasticsearch.FSharp.DSL

let sortModeToJson (sortMode:SortMode) = 
    match sortMode with 
    | SortMode.Min -> "\"min\""
    | SortMode.Max -> "\"max\""
    | SortMode.Sum -> "\"sum\""
    | SortMode.Avg -> "\"avg\""
    | SortMode.Median -> "\"median\""
    
let sortOrderToJson sortOrder =
    match sortOrder with
    | SortOrder.Asc -> "\"asc\""
    | SortOrder.Desc -> "\"desc\""
    
let internal sortFieldListToJson (sortFieldList : SortField list ) = 
    "{" + 
        ([
            for field in sortFieldList ->  
                match field with 
                | Order orderVal -> 
                    "\"order\":" + (sortOrderToJson orderVal)
                | Mode modeVal -> 
                    "\"mode\":" + (sortModeToJson modeVal)
        ] |> String.concat ",")
    + "}"
    
let internal sortBodyListToJson (sortBody:(string * (SortField list)) list) = 
    "[" +
        ([
            for (name, fields) in sortBody -> 
                let body = sortFieldListToJson fields 
                "{\"" + name + "\":" + body + "}"
        ] |> String.concat ",")
    + "]"
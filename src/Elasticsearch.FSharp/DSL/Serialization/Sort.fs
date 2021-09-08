module internal Elasticsearch.FSharp.DSL.Serialization.Sort

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.Utility

type SortMode with
    member x.ToJson() =
        match x with 
        | SortMode.Min -> Json.quoteString "min"
        | SortMode.Max -> Json.quoteString "max"
        | SortMode.Sum -> Json.quoteString "sum"
        | SortMode.Avg -> Json.quoteString "avg"
        | SortMode.Median -> Json.quoteString "median"
    
type SortOrder with
    member x.ToJson() =
        match x with
        | SortOrder.Asc -> Json.quoteString "asc"
        | SortOrder.Desc -> Json.quoteString "desc"
        
type SortField with
    member x.ToJson() =
        match x with 
        | Order order ->
            Json.makeKeyValue "order" (order.ToJson())
        | Mode mode -> 
            Json.makeKeyValue "mode" (mode.ToJson())

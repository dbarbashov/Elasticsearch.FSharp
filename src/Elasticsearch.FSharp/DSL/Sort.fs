namespace Elasticsearch.FSharp.DSL

type SortBody = string * (SortField list)

and SortField = 
    | Order of SortOrder
    | Mode of SortMode
    
and
    [<RequireQualifiedAccess>]
    SortOrder =
    | Asc
    | Desc

and 
    [<RequireQualifiedAccess>]
    SortMode =
    | Min
    | Max
    | Sum 
    | Avg
    | Median
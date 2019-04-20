namespace Elasticsearch.FSharp.DSL

type RangeQuery = string * (RangeQueryField list)

and RangeQueryField = 
    | Gte of string
    | Gt of string 
    | Lte of string 
    | Lt of string
    | RangeTimeZone of string
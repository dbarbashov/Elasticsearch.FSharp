namespace Elasticsearch.FSharp.DSL

type MultiMatchQuery = MultiMatchQueryField list

and MultiMatchQueryField =
    | Fields of string list
    | MultiMatchQuery of string
    | QueryType of string
    | MaxExpansions of int
    | Slop of int
    | TieBreaker of float
    | MultiMatchRaw of Key: string * Value: string


namespace Elasticsearch.FSharp.DSL

type MatchPhrasePrefixQuery = string * (MatchPhrasePrefixQueryField list)

and MatchPhrasePrefixQueryField =
    | MatchQuery of string
    | MaxExpansions of int
    | Slop of int
    | Analyzer of string
    | Boost of float

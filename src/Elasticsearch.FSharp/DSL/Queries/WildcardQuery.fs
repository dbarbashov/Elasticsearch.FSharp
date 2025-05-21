namespace Elasticsearch.FSharp.DSL

type WildcardQuery = string * (WildcardQueryField list)

and WildcardQueryField =
    | PatternValue of string
    | Rewrite of RewriteOption
    | Boost of float

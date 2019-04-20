namespace Elasticsearch.FSharp.DSL
    
type MatchQuery = string * (MatchQueryField list)

and MatchQueryField =
    | Operator of string
    | ZeroTermsQuery of string
    | MatchQuery of string
    | CutoffFrequency of double 
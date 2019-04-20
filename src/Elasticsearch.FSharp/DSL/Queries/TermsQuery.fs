namespace Elasticsearch.FSharp.DSL

type TermsQuery = string * (TermsQueryField list)
    
and TermsQueryField = 
    | ValueList of string list
    | FromIndex of string * string * string * string
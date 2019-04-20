namespace Elasticsearch.FSharp.DSL

type TermQuery = string * (TermQueryField list)

and TermQueryField = 
    | ExactValue of string 


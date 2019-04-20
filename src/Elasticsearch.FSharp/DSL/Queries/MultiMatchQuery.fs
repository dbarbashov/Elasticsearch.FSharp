namespace Elasticsearch.FSharp.DSL

type MultiMatchQuery = MultiMatchQueryField list

and MultiMatchQueryField =
    | Fields of string list
    | MultiMatchQuery of string


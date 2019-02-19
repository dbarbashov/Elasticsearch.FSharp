#r "bin/Debug/Elasticsearch.FSharp.dll"
open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL

let query = 
    Search [
        Query ( MatchAll )
        Source_ Nothing
        Source_ (Only "123")
        Source_ (List ["1"; "2"; "3"])
        Source_ (Pattern (["s*"], ["sl*"])) 
    ]

printf "%A\n" (query |> ElasticDSLToJson)

#quit
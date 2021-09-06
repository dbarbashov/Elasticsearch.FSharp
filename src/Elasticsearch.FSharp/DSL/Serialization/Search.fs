module internal Elasticsearch.FSharp.DSL.Serialization.Search

open Elasticsearch.FSharp.DSL

let ElasticDSLToJson (Search elasticBody:ElasticDSL) =
    "{" + 
        ([
            for searchBody in elasticBody ->  
                match searchBody with 
                | Query queryBody -> 
                    let body = Query.queryBodyToJson queryBody
                    "\"query\":" + body
                | Sort sortBody ->
                    let body = Sort.sortBodyListToJson sortBody
                    "\"sort\":" + body
                | ScriptFields fields -> 
                    let body = Script.scriptFieldsBodyToJson fields
                    "\"script_fields\":{" + body + "}"
                | Aggs fields -> 
                    let body = Aggs.AggsBodyToJSON fields
                    "\"aggs\":{" + body + "}"
                | From x -> 
                    "\"from\":" + x.ToString()
                | Size x -> 
                    "\"size\":" + x.ToString()
                | Source_ x ->
                    "\"_source\":" + Source.sourceBodyToJson x
                | Raw (key, value) ->
                    "\""+key+"\":" + value
        ] |> String.concat ",")
    + "}"
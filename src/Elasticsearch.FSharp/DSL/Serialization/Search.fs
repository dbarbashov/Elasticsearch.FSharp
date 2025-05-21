module internal Elasticsearch.FSharp.DSL.Serialization.Search

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization.Query
open Elasticsearch.FSharp.Utility
open Elasticsearch.FSharp.DSL.Serialization.Sort
open Elasticsearch.FSharp.DSL.Serialization.Source

type SearchBody with
    member x.ToJson() = 
        match x with
        | Query queryBody -> 
            let body = Query.queryBodyToJson queryBody
            Json.makeKeyValue "query" body
        | Sort sortBody ->
            Json.makeKeyValue "sort" (Json.makeArray [
                for name, fields in sortBody ->
                    Json.makeObject [
                        Json.makeKeyValue name (Json.makeObject [
                            for field in fields ->
                                field.ToJson()
                        ])
                    ]
            ])
        | ScriptFields fields -> 
            Json.makeKeyValue "script_fields" (ScriptField.scriptFieldsBodyToJson fields)
        | Script script ->
            Script.scriptToJson script
        | Aggs fields ->
            let body = Aggs.aggsBodyToJson fields
            Json.makeKeyValue "aggs" body
        | From x -> 
            Json.makeKeyValue "from" (x.ToString())
        | Size x -> 
            Json.makeKeyValue "size" (x.ToString())
        | Source_ x ->
            Json.makeKeyValue "_source" (x.ToJson())
        | Raw (key, value) ->
            Json.makeKeyValue key value
        | TrackTotalHits x ->
            Json.makeKeyValue "track_total_hits" (Json.boolToString x)
        
type ElasticDSL with
    member x.ToJson() =
        match x with
        | Search bodies ->
            Json.makeObject [
                for body in bodies ->
                    body.ToJson()
            ]
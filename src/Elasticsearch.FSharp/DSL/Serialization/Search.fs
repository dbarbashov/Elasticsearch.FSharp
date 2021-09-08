module internal Elasticsearch.FSharp.DSL.Serialization.Search

open Elasticsearch.FSharp.DSL
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
            let body = Script.scriptFieldsBodyToJson fields
            Json.makeKeyValue "script_fields" $"{{{body}}}"
        | Aggs fields -> 
            let body = Aggs.AggsBodyToJSON fields
            Json.makeKeyValue "aggs" $"{{{body}}}"
        | From x -> 
            Json.makeKeyValue "from" (x.ToString())
        | Size x -> 
            Json.makeKeyValue "size" (x.ToString())
        | Source_ x ->
            Json.makeKeyValue "_source" (x.ToJson())
        | Raw (key, value) ->
            Json.makeKeyValue key value
        
type ElasticDSL with
    member x.ToJson() =
        match x with
        | Search bodies ->
            Json.makeObject [
                for body in bodies ->
                    body.ToJson()
            ]
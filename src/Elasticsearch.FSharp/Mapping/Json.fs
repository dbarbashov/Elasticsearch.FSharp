module Elasticsearch.FSharp.Mapping.Json

open Elasticsearch.FSharp.Mapping.DSL
open Elasticsearch.FSharp.Utility

type MappingSetting with 
    member x.ToJson() =
        match x with
        | Setting value ->
            Json.quoteString (value.ToString())
        | BoolSetting value ->
            Json.boolToString value

type PropertyMapping with
    member x.ToJson() = Json.makeObject [
        if not x.Enabled then 
            yield Json.makeKeyValue "enabled" (Json.boolToString x.Enabled)
        
        match x.Type with
        | Some t -> yield Json.makeKeyValue "type" (Json.quoteString t)
        | None -> ()
        
        match x.Analyzer with
        | Some a -> yield Json.makeKeyValue "analyzer" (Json.quoteString a)
        | None -> ()
        
        match x.Format with
        | Some f -> yield Json.makeKeyValue "format" (Json.quoteString f)
        | None -> ()
        
        match x.Properties with
        | Some props ->
            yield Json.makeKeyValue "properties" (Json.makeObject [
                for p in props do
                    yield Json.makeKeyValue p.Key (p.Value.ToJson())
            ])
        | None -> ()
        
        match x.Fields with
        | Some fields ->
            yield Json.makeKeyValue "fields" (Json.makeObject [
                for f in fields do
                    yield Json.makeKeyValue f.Key (f.Value.ToJson())
            ])
        | None -> ()
    ]

type TypeMapping with
    member x.ToJson() = Json.makeObject [
        match x.AllEnabled with
        | Some true ->
            yield
                Json.makeKeyValue "_all" (Json.makeObject [
                    Json.makeKeyValue "enabled" "true"
                ])
        | _ -> ()
        
        yield Json.makeKeyValue "properties" (Json.makeObject [
            for p in x.Properties do
                yield Json.makeKeyValue p.Key (p.Value.ToJson())
        ])
    ]
    
type ElasticMapping with
    member x.ToJson() = Json.makeObject [
        match x.Settings with
        | Some settings -> 
            yield Json.makeKeyValue "settings" (Json.makeObject [
                for s in settings ->
                    Json.makeKeyValue s.Key (s.Value.ToJson())
            ])
        | None -> ()
        
        yield Json.makeKeyValue "mappings" (Json.makeObject [
            Json.makeKeyValue "_doc" (x.Mappings.ToJson())
        ])
    ]
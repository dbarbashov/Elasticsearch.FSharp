module Elasticsearch.FSharp.Mapping.Json

open System
open System.Runtime.InteropServices
open Elasticsearch.FSharp.Mapping.DSL
open Elasticsearch.FSharp.Utility

let [<Literal>] private OpenSearchFlatObject = "flat_object"

type MappingSetting with 
    member x.ToJson() =
        match x with
        | Setting value ->
            Json.quoteString (value.ToString())
        | BoolSetting value ->
            Json.boolToString value

type PropertyMapping with
    member x.ToJson([<Optional; DefaultParameterValue(false)>] usingOpenSearch) = Json.makeObject [
        if not x.Enabled then 
            yield Json.makeKeyValue "enabled" (Json.boolToString x.Enabled)
            
        if x.IgnoreMalformed then
            yield Json.makeKeyValue "ignore_malformed" (Json.boolToString x.IgnoreMalformed)

        let ``type`` =
            x.Type
            |> Option.map(fun t -> if usingOpenSearch && t = "flattened" then OpenSearchFlatObject else t)
            |> Option.defaultValue ""

        if ``type`` |> String.IsNullOrEmpty |> not then
            yield Json.makeKeyValue "type" (Json.quoteString ``type``)
            
        match x.IgnoreAbove with
        | Some ia ->
            if ``type``<> OpenSearchFlatObject then
                yield Json.makeKeyValue "ignore_above" (Json.uintToString ia)
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
                    yield Json.makeKeyValue p.Key (p.Value.ToJson(usingOpenSearch))
            ])
        | None -> ()
        
        match x.Fields with
        | Some fields ->
            yield Json.makeKeyValue "fields" (Json.makeObject [
                for f in fields do
                    yield Json.makeKeyValue f.Key (f.Value.ToJson(usingOpenSearch))
            ])
        | None -> ()
    ]

type TypeMapping with
    member x.ToJson([<Optional; DefaultParameterValue(false)>] usingOpenSearch) = Json.makeObject [
        match x.AllEnabled with
        | Some true ->
            yield
                Json.makeKeyValue "_all" (Json.makeObject [
                    Json.makeKeyValue "enabled" "true"
                ])
        | _ -> ()
        
        yield Json.makeKeyValue "properties" (Json.makeObject [
            for p in x.Properties do
                yield Json.makeKeyValue p.Key (p.Value.ToJson(usingOpenSearch))
        ])
    ]
    
type ElasticMapping with
    member x.ToJson(
        [<Optional; DefaultParameterValue(true)>] includeTypeName,
        [<Optional; DefaultParameterValue(false)>] usingOpenSearch) = Json.makeObject [

        match x.Settings with
        | Some settings -> 
            yield Json.makeKeyValue "settings" (Json.makeObject [
                for s in settings ->
                    Json.makeKeyValue s.Key (s.Value.ToJson())
            ])
        | None -> ()
        
        if includeTypeName then 
            yield Json.makeKeyValue "mappings" (Json.makeObject [
                Json.makeKeyValue "_doc" (x.Mappings.ToJson(usingOpenSearch))
            ])
        else
            yield Json.makeKeyValue "mappings" (x.Mappings.ToJson(usingOpenSearch))
    ]
    
    member x.ToPutMappingsJson([<Optional; DefaultParameterValue(false)>] usingOpenSearch) = [|
        for kv in x.Mappings.Properties do
            yield Json.makeObject [
                Json.makeKeyValue "properties" (Json.makeObject [
                    Json.makeKeyValue kv.Key (kv.Value.ToJson(usingOpenSearch))
                ])
            ]
    |]
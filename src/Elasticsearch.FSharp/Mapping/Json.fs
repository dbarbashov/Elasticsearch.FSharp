module Elasticsearch.FSharp.Mapping.Json

open Elasticsearch.FSharp.Mapping.DSL

let inline internal quoteString (s: string) = $"\"{s}\""
let inline internal boolToString (b: bool) = if b then "true" else "false"
let inline internal makeKeyValue (key: string) (value: string) = $"{quoteString key}:{value}"
let inline internal makeObject innerParts =
    let middle = innerParts |> String.concat ","
    $"{{{middle}}}"

type MappingSetting with 
    member x.ToJson() =
        match x with
        | Setting value ->
            quoteString (value.ToString())
        | BoolSetting value ->
            boolToString value

type PropertyMapping with
    member x.ToJson() = makeObject [
        if not x.Enabled then 
            yield makeKeyValue "enabled" (boolToString x.Enabled)
        
        match x.Type with
        | Some t -> yield makeKeyValue "type" (quoteString t)
        | None -> ()
        
        match x.Analyzer with
        | Some a -> yield makeKeyValue "analyzer" (quoteString a)
        | None -> ()
        
        match x.Format with
        | Some f -> yield makeKeyValue "format" (quoteString f)
        | None -> ()
        
        match x.Properties with
        | Some props ->
            yield makeKeyValue "properties" (makeObject [
                for p in props do
                    yield makeKeyValue p.Key (p.Value.ToJson())
            ])
        | None -> ()
        
        match x.Fields with
        | Some fields ->
            yield makeKeyValue "fields" (makeObject [
                for f in fields do
                    yield makeKeyValue f.Key (f.Value.ToJson())
            ])
        | None -> ()
    ]

type TypeMapping with
    member x.ToJson() = makeObject [
        match x.AllEnabled with
        | Some true ->
            yield
                makeKeyValue "_all" (makeObject [
                    makeKeyValue "enabled" "true"
                ])
        | _ -> ()
        
        yield makeKeyValue "properties" (makeObject [
            for p in x.Properties do
                yield makeKeyValue p.Key (p.Value.ToJson())
        ])
    ]
    
type ElasticMapping with
    member x.ToJson() = makeObject [
        match x.Settings with
        | Some settings -> 
            yield makeKeyValue "settings" (makeObject [
                for s in settings ->
                    makeKeyValue s.Key (s.Value.ToJson())
            ])
        | None -> ()
        
        yield makeKeyValue "mappings" (makeObject [
            makeKeyValue "_doc" (x.Mappings.ToJson())
        ])
    ]
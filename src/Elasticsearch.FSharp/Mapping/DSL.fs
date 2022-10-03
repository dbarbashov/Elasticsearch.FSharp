module Elasticsearch.FSharp.Mapping.DSL

open System.Collections.Generic

type MappingSetting =
    | Setting of Value: string
    | BoolSetting of Value: bool

type PropertyMapping = {
    Type: string option
    Analyzer: string option
    Enabled: bool
    IgnoreMalformed: bool
    Format: string option
    Properties: Dictionary<string, PropertyMapping> option
    Fields: Dictionary<string, PropertyMapping> option
}
with
    static member Default = {
        Type = None
        Analyzer = None
        Enabled = true
        IgnoreMalformed = false
        Format = None
        Properties = None
        Fields = None
    }

type TypeMapping = {
    AllEnabled: bool option
    Properties: Dictionary<string, PropertyMapping>
}

type ElasticMapping = {
    Settings: Dictionary<string, MappingSetting> option
    Mappings: TypeMapping
}
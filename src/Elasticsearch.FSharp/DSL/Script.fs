namespace Elasticsearch.FSharp.DSL

type ScriptFieldsBody = string * ScriptField list

and ScriptField = 
    | Source of string
    | Lang of string
    | Params of (string * string) list
    | ScriptId of string

type ScriptBody = Script list
    
and Script =
    | Source of string
    | Lang of string
    | Params of (string * string) list
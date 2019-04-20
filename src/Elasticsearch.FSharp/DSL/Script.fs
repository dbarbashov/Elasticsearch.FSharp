namespace Elasticsearch.FSharp.DSL

type ScriptFieldsBody = string * (ScriptField list)

and ScriptField = 
    | Source of string
    | Lang of string
    | Params of (string * string) list
    | ScriptId of string
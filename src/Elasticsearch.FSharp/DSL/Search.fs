namespace Elasticsearch.FSharp.DSL

type ElasticDSL = 
    | Search of SearchBody list

and SearchBody = 
    | Query of QueryBody
    | Sort of SortBody list
    | ScriptFields of ScriptFieldsBody list
    | Aggs of AggsFieldsBody list
    | From of int
    | Size of int
    | Source_ of SourceBody
    | Raw of key:string * value:string
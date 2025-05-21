namespace Elasticsearch.FSharp.DSL

// QueryBody is defined in Elasticsearch.FSharp.DSL.Query
// ScoreModeOption is defined in Elasticsearch.FSharp.DSL.Queries.ScoreModeOption

type NestedQuery = NestedQueryField list

and NestedQueryField =
    | Path of string
    | Query of QueryBody
    | ScoreMode of ScoreModeOption
    | IgnoreUnmapped of bool

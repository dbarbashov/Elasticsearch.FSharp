namespace Elasticsearch.FSharp.DSL

type ScoreModeOption =
    | Avg
    | Max
    | Min
    | NoneScore // "None" is a keyword, so "NoneScore" is used
    | Sum

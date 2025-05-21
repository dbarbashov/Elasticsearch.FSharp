namespace Elasticsearch.FSharp.DSL

type ScoreModeOption =
    | ScoreModeAvg
    | ScoreModeMax
    | ScoreModeMin
    | ScoreModeNone // "None" is a keyword, so "NoneScore" is used
    | ScoreModeSum

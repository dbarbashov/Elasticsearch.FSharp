namespace Elasticsearch.FSharp.DSL

type RewriteOption =
    | ConstantScoreBlended
    | ConstantScore
    | ConstantScoreBoolean
    | ScoringBoolean
    | TopTermsBlendedFreqs of int
    | TopTermsBoost of int
    | TopTerms of int
    member this.ToStringValue() =
        match this with
        | ConstantScoreBlended -> "constant_score_blended"
        | ConstantScore -> "constant_score"
        | ConstantScoreBoolean -> "constant_score_boolean"
        | ScoringBoolean -> "scoring_boolean"
        | TopTermsBlendedFreqs n -> sprintf "top_terms_blended_freqs_%d" n
        | TopTermsBoost n -> sprintf "top_terms_boost_%d" n
        | TopTerms n -> sprintf "top_terms_%d" n

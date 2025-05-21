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

type QueryBody = 
    | MatchAll
    | MatchNone
    | Ids of string list
    | Bool of BoolQuery list 
    | Match of MatchQuery
    | Term of TermQuery
    | Terms of TermsQuery
    | Range of RangeQuery
    | Wildcard of WildcardQuery
    | Script of ScriptBody
    | MultiMatch of MultiMatchQuery
    | MatchPhrasePrefix of MatchPhrasePrefixQuery
    | Exists of FieldName: string
    | Raw of RawString: string
    | TypeEquals of string
    
and BoolQuery = 
    | Must of QueryBody list
    | Filter of QueryBody list
    | Should of QueryBody list
    | MustNot of QueryBody list
    | MinimumShouldMatch of string

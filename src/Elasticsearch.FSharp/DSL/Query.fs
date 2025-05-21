namespace Elasticsearch.FSharp.DSL

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
    | Nested of NestedQuery // Added Nested query type
    | Exists of FieldName: string
    | Raw of RawString: string
    | TypeEquals of string
    
and BoolQuery = 
    | Must of QueryBody list
    | Filter of QueryBody list
    | Should of QueryBody list
    | MustNot of QueryBody list
    | MinimumShouldMatch of string

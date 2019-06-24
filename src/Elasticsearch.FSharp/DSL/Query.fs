namespace Elasticsearch.FSharp.DSL

type QueryBody = 
    | MatchAll
    | MatchNone
    | Bool of BoolQuery list 
    | Match of MatchQuery
    | Term of TermQuery
    | Terms of TermsQuery
    | Range of RangeQuery
    | Script of ScriptQuery
    | MultiMatch of MultiMatchQuery
    | MatchPhrasePrefix of MatchPhrasePrefixQuery
    | Exists of fieldName:string
    | Raw of rawString:string
    | TypeEquals of string
    
and BoolQuery = 
    | Must of QueryBody list
    | Filter of QueryBody list
    | Should of QueryBody list
    | MustNot of QueryBody list
    | MinimumShouldMatch of string
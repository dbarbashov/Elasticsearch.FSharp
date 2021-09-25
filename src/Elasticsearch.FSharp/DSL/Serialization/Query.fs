module internal rec Elasticsearch.FSharp.DSL.Serialization.Query

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization.Queries
open Elasticsearch.FSharp.Utility

type BoolQuery with
    member x.ToJson() =
        match x with 
        | Must queryBody ->
            Json.makeKeyValue "must" (queryBodyListToJsonBool queryBody) 
        | Filter queryBody ->
            Json.makeKeyValue "filter" (queryBodyListToJsonBool queryBody) 
        | Should queryBody ->
            Json.makeKeyValue "should" (queryBodyListToJsonBool queryBody) 
        | MustNot queryBody ->
            Json.makeKeyValue "must_not" (queryBodyListToJsonBool queryBody) 
        | MinimumShouldMatch x ->
            Json.makeKeyValue "minimum_should_match" (Json.quoteString (x.ToString()))

let boolQueryToJson (boolQueryBody: BoolQuery seq) =
    Json.makeObject [
        for boolPart in boolQueryBody ->
            boolPart.ToJson()
    ]
    
type QueryBody with
    member x.ToJson() =
        match x with 
        | MatchAll ->
            Json.makeKeyValue "match_all" (Json.makeObject [])
        | MatchNone ->
            Json.makeKeyValue "match_none" (Json.makeObject [])
        | Ids ids ->
            Json.makeKeyValue "ids" (Json.makeObject [
                Json.makeKeyValue "values" (Json.makeQuotedArray ids)
            ])
        | Bool boolQuery ->
            Json.makeKeyValue "bool" (boolQueryToJson boolQuery)
        | Match matchQuery ->
            Json.makeKeyValue "match" (MatchQuery.matchQueryToJson matchQuery)
        | Term termQuery -> 
            Json.makeKeyValue "term" (TermQuery.termQueryToJson termQuery)
        | Terms termsQuery ->
            Json.makeKeyValue "terms" (TermsQuery.termsQueryToJson termsQuery)
        | Range rangeQuery ->
            Json.makeKeyValue "range" (RangeQuery.rangeQueryToJson rangeQuery)
        | Script scriptQuery -> 
            Json.makeKeyValue "script" (ScriptQuery.scriptQueryToJson scriptQuery)
        | MultiMatch multimatchBody ->
            Json.makeKeyValue "multi_match" (MultiMatchQuery.multimatchBodyToJson multimatchBody)
        | MatchPhrasePrefix matchPhrasePrefixBody ->
            Json.makeKeyValue "match_phrase_prefix" (MatchPhrasePrefixQuery.matchPhrasePrefixQueryToJson matchPhrasePrefixBody)
        | Exists field ->
            Json.makeKeyValue "exists" (Json.makeObject [
                Json.makeKeyValue "field" (Json.quoteString field)
            ])
        | TypeEquals t ->
            Json.makeKeyValue "type" (Json.makeObject [
                Json.makeKeyValue "value" (Json.quoteString t)
            ])
        | QueryBody.Raw body ->
            body
    
let queryBodyToJson (queryPart: QueryBody) =
    Json.makeObject [
        queryPart.ToJson()
    ]

let queryBodyListToJsonBool (queryBody: QueryBody list) =
    Json.makeArray [
        for body in queryBody ->
            queryBodyToJson body
    ]
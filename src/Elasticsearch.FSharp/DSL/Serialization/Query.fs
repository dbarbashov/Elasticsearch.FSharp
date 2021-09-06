module internal rec Elasticsearch.FSharp.DSL.Serialization.Query

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization.Queries

let boolQueryToJson boolQueryBody = 
    "{" + 
        ([
            for boolPart in boolQueryBody ->
                 match boolPart with 
                 | Must queryBody ->
                    let body = queryBodyListToJsonBool queryBody 
                    "\"must\":" + body
                 | Filter queryBody ->
                    let body = queryBodyListToJsonBool queryBody 
                    "\"filter\":" + body
                 | Should queryBody ->
                    let body = queryBodyListToJsonBool queryBody 
                    "\"should\":" + body
                 | MustNot queryBody ->
                    let body = queryBodyListToJsonBool queryBody 
                    "\"must_not\":" + body
                 | MinimumShouldMatch x -> 
                    "\"minimum_should_match\":\"" + x.ToString() + "\""
        ] |> String.concat ",")
    + "}"
    
let queryBodyToJson (queryPart: QueryBody) =
    "{" + 
        match queryPart with 
        | MatchAll -> 
            "\"match_all\":{}"
        | MatchNone ->
            "\"match_none\":{}"
        | IDs ids ->
            "\"ids\":" + "{\"values\":[" + (ids |> List.map (fun x -> "\"" + x + "\"") |> String.concat ",") + "]}"
        | Bool boolQuery ->
            let body = boolQueryToJson boolQuery
            "\"bool\":" + body
        | Match matchQuery ->
            let body = MatchQuery.matchQueryToJson matchQuery 
            "\"match\":" + body
        | Term termQuery -> 
            let body = TermQuery.termQueryToJson termQuery
            "\"term\":" + body
        | Terms termsQuery ->
            let body = TermsQuery.termsQueryToJson termsQuery
            "\"terms\":" + body
        | Range rangeQuery ->
            let body = RangeQuery.rangeQueryToJson rangeQuery 
            "\"range\":" + body
        | Script scriptQuery -> 
            let body = ScriptQuery.ScriptQueryToJson scriptQuery
            "\"script\":" + body
        | MultiMatch multimatchBody ->
            let body = MultiMatchQuery.multimatchBodyToJson multimatchBody
            "\"multi_match\":" + body
        | MatchPhrasePrefix matchPhrasePrefixBody ->
            let body = MatchPhrasePrefixQuery.matchPhrasePrefixQueryToJson matchPhrasePrefixBody
            "\"match_phrase_prefix\":" + body
        | Exists field ->
            "\"exists\":{\"field\":\"" + field + "\"}"
        | TypeEquals t ->
            "\"type\":{\"value\":\"" + t + "\"}"
        | QueryBody.Raw body ->
            body
    + "}"
    
let queryBodyListToJsonList = List.map queryBodyToJson  

let queryBodyListToJsonBool (queryBody: QueryBody list) = 
    "[" + ( queryBodyListToJsonList queryBody |> String.concat "," ) + "]"
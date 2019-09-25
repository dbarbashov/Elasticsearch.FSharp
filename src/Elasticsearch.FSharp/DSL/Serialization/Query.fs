module internal rec Elasticsearch.FSharp.DSL.Serialization.Query

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization.Queries

let BoolQueryToJson boolQueryBody = 
    "{" + 
        ([
            for boolPart in boolQueryBody ->
                 match boolPart with 
                 | Must queryBody ->
                    let body = QueryBodyListToJsonBool queryBody 
                    "\"must\":" + body
                 | Filter queryBody ->
                    let body = QueryBodyListToJsonBool queryBody 
                    "\"filter\":" + body
                 | Should queryBody ->
                    let body = QueryBodyListToJsonBool queryBody 
                    "\"should\":" + body
                 | MustNot queryBody ->
                    let body = QueryBodyListToJsonBool queryBody 
                    "\"must_not\":" + body
                 | MinimumShouldMatch x -> 
                    "\"minimum_should_match\":\"" + x.ToString() + "\""
        ] |> String.concat ",")
    + "}"
    
let QueryBodyToJson (queryPart: QueryBody) =
    "{" + 
        match queryPart with 
        | MatchAll -> 
            "\"match_all\":{}"
        | MatchNone ->
            "\"match_none\":{}"
        | IDs ids ->
            "\"ids\":" + "{\"values\":[" + (ids |> List.map (fun x -> "\"" + x + "\"") |> String.concat ",") + "]}"
        | Bool boolQuery ->
            let body = BoolQueryToJson boolQuery
            "\"bool\":" + body
        | Match matchQuery ->
            let body = MatchQuery.MatchQueryToJson matchQuery 
            "\"match\":" + body
        | Term termQuery -> 
            let body = TermQuery.TermQueryToJson termQuery
            "\"term\":" + body
        | Terms termsQuery ->
            let body = TermsQuery.TermsQueryToJson termsQuery
            "\"terms\":" + body
        | Range rangeQuery ->
            let body = RangeQuery.RangeQueryToJson rangeQuery 
            "\"range\":" + body
        | Script scriptQuery -> 
            let body = ScriptQuery.ScriptQueryToJson scriptQuery
            "\"script\":" + body
        | MultiMatch multimatchBody ->
            let body = MultiMatchQuery.MultimatchBodyToJson multimatchBody
            "\"multi_match\":" + body
        | MatchPhrasePrefix matchPhrasePrefixBody ->
            let body = MatchPhrasePrefixQuery.MatchPhrasePrefixQueryToJson matchPhrasePrefixBody
            "\"match_phrase_prefix\":" + body
        | Exists field ->
            "\"exists\":{\"field\":\"" + field + "\"}"
        | TypeEquals t ->
            "\"type\":{\"value\":\"" + t + "\"}"
        | QueryBody.Raw body ->
            body
    + "}"
    
let QueryBodyListToJsonList = List.map QueryBodyToJson  

let QueryBodyListToJsonBool (queryBody: QueryBody list) = 
    "[" + ( QueryBodyListToJsonList queryBody |> String.concat "," ) + "]"
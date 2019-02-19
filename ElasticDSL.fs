module rec Elasticsearch.FSharp.DSL
    
[<AutoOpen>]
module QueryDSL = 
    type QueryBody = 
        | MatchAll
        | MatchNone
        | Bool of BoolQuery list 
        | Match of MatchQuery
        | Term of TermQuery
        | Terms of TermsQuery
        | Range of RangeQuery
        | Script of ScriptQuery
        | MultiMatch of MultiMatchQueryField list
        | Exists of string
        | Raw of string
        | TypeEquals of string
        
    and BoolQuery = 
        | Must of QueryBody list
        | Filter of QueryBody list
        | Should of QueryBody list
        | MustNot of QueryBody list
        | MinimumShouldMatch of string
        
    and MatchQuery = string * (MatchQueryField list)
    and TermQuery = string * (TermQueryField list)
    and TermsQuery = string * (TermsQueryField list)
    and RangeQuery = string * (RangeQueryField list)
    and ScriptQuery = ScriptQueryField list
    and MultiMatchQueryField =
        | Fields of string list
        | MultiMatchQuery of string
        
    and MatchQueryField =
        | Operator of string
        | ZeroTermsQuery of string
        | MatchQuery of string
        | CutoffFrequency of double 
        
    and TermQueryField = 
        | ExactValue of string 
        
    and TermsQueryField = 
        | ValueList of string list
        | FromIndex of string * string * string * string
        
    and RangeQueryField = 
        | Gte of string
        | Gt of string 
        | Lte of string 
        | Lt of string
        | RangeTimeZone of string
        
    and ScriptQueryField = 
        | Source of string
        | Lang of string
        | Params of (string * string) list
        | ScriptId of string
        
    let internal BoolQueryToJson boolQueryBody = 
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
        
    let internal MatchQueryToJson ((name, matchBody) : string * (MatchQueryField list) ) = 
        "{" + 
            (
                "\"" + name + "\":{" + 
                    ([
                        for matchParam in matchBody ->
                            match matchParam with 
                            | Operator x -> 
                                "\"operator\":\"" + x + "\""
                            | MatchQuery x ->
                                if x = null then 
                                    "\"query\":null"
                                else 
                                    "\"query\":\"" + x + "\""
                            | CutoffFrequency x -> 
                                "\"cutoff_frequency\":" + x.ToString()
                            | ZeroTermsQuery x -> 
                                "\"zero_terms_query\":" + x.ToString()
                    ] |> String.concat ",")
                + "}"
            )
        + "}"
        
    let internal TermQueryToJson ((name, termBody) : string * (TermQueryField list) ) = 
        "{" + 
            (
                "\"" + name + "\":" + 
                    ([
                        for termParam in termBody->
                            match termParam with 
                            | ExactValue x ->
                                if x = null then
                                    "null"
                                else
                                    "\"" + x + "\""
                    ] |> String.concat ",")
            )
        + "}"
        
    let internal TermsQueryToJson ((name, termsBody) : string * (TermsQueryField list)) =     
        "{" + 
            (
                "\"" + name + "\":" + 
                    (
                        match termsBody with 
                        | [ ValueList x ] -> 
                            "[" + (x |> List.map (fun x -> "\"" + x + "\"") |> String.concat ",") + "]"
                        | [ FromIndex (index, _type, id, path) ] -> 
                            "{" + 
                            "\"index\":\"" + index + "\"," +
                            "\"type\":\"" + _type + "\"," +
                            "\"id\":\"" + id + "\"," +
                            "\"path\":\"" + path+ "\"" +
                            "}" 
                        | _ ->
                            "_error_"
                    )
            )
        + "}"
        
    let internal RangeQueryToJson ((name, rangeBody): string * (RangeQueryField list) ) = 
        "{" + 
            (
                "\"" + name + "\":{" + 
                    ([
                        for rangeParam in rangeBody ->
                            match rangeParam with 
                            | Gte x -> 
                                "\"gte\":\"" + x + "\""
                            | Gt x -> 
                                "\"gt\":\"" + x + "\""
                            | Lte x -> 
                                "\"lte\":\"" + x + "\""
                            | Lt x -> 
                                "\"lt\":\"" + x + "\""
                            | RangeTimeZone x -> 
                                "\"time_zone\":\"" + x + "\""
                    ] |> String.concat ",")
                + "}"
            )
        + "}"
        
    let internal ScriptQueryBodyToJSON (scriptBody:ScriptQueryField list) =
        ([
            for rangeParam in scriptBody ->
                match rangeParam with 
                | Source x -> 
                    "\"source\":\"" + x + "\""
                | Lang x -> 
                    "\"lang\":\"" + x + "\""
                | ScriptId x -> 
                    "\"id\":\"" + x + "\""
                | Params x -> 
                    let x = x |> List.map (fun (k, v) -> "\"" + k + "\":" + "\"" + v + "\"") |> String.concat ","
                    "\"params\":{" + x + "}"
        ] |> String.concat ",") 
        
    let internal ScriptQueryToJson (scriptBody:ScriptQueryField list) = 
            "{" + 
                (
                    "\"script\":{" + (ScriptQueryBodyToJSON scriptBody) + "}"
                )
            + "}"
        
    let internal MultimatchBodyToJson multimatchBody =
        "{" +
            ([
                 for field in multimatchBody do
                     match field with
                     | Fields fieldList ->
                         yield ("\"fields\":[" + (List.map (fun s -> "\"" + s + "\"") fieldList |> String.concat ",") + "]")
                     | MultiMatchQuery query ->
                         if query = null then
                             yield "\"query\":null"
                         else 
                             yield "\"query\":\"" + query + "\""
            ] |> String.concat ",")
        + "}"
        
    let QueryBodyToJson (queryPart: QueryBody) =
        "{" + 
            match queryPart with 
            | MatchAll -> 
                "\"match_all\":{}"
            | MatchNone ->
                "\"match_none\":{}"
            | Bool boolQuery ->
                let body = BoolQueryToJson boolQuery
                "\"bool\":" + body
            | Match matchQuery ->
                let body = MatchQueryToJson matchQuery 
                "\"match\":" + body
            | Term termQuery -> 
                let body = TermQueryToJson termQuery
                "\"term\":" + body
            | Terms termsQuery ->
                let body = TermsQueryToJson termsQuery
                "\"terms\":" + body
            | Range rangeQuery ->
                let body = RangeQueryToJson rangeQuery 
                "\"range\":" + body
            | Script scriptQuery -> 
                let body = ScriptQueryToJson scriptQuery
                "\"script\":" + body
            | MultiMatch multimatchBody ->
                let body = MultimatchBodyToJson multimatchBody
                "\"multi_match\":" + body
            | Exists field ->
                "\"exists\":{\"field\":\"" + field + "\"}"
            | TypeEquals t ->
                "\"type\":{\"value\":\"" + t + "\"}"
            | Raw body ->
                body
        + "}"
        
    let internal QueryBodyListToJsonList = List.map QueryBodyToJson  
    
    let internal QueryBodyListToJsonBool (queryBody: QueryBody list) = 
        "[" + ( QueryBodyListToJsonList queryBody |> String.concat "," ) + "]"

[<AutoOpen>]
module SortDSL =
    type SortBody = string * (SortField list)
    
    and SortField = 
        | Order of SortOrder
        | Mode of SortMode
        
    and SortOrder =
        | Asc
        | Desc
    
    and SortMode =
        | Min
        | Max
        | Sum 
        | Avg
        | Median
        
    let internal SortModeToJson sortMode = 
        match sortMode with 
        | Min -> "\"min\""
        | Max -> "\"max\""
        | Sum -> "\"sum\""
        | Avg -> "\"avg\""
        | Median -> "\"median\""
        
    let internal SortOrderToJson sortOrder =
        match sortOrder with
        | Asc -> "\"asc\""
        | Desc -> "\"Desc\""
        
    let internal SortFieldListToJson (sortFieldList : SortField list ) = 
        "{" + 
            ([
                for field in sortFieldList ->  
                    match field with 
                    | Order orderVal -> 
                        "\"order\":" + (SortOrderToJson orderVal)
                    | Mode modeVal -> 
                        "\"mode\":" + (SortModeToJson modeVal)
            ] |> String.concat ",")
        + "}"
        
    let internal SortBodyListToJson (sortBody:(string * (SortField list)) list) = 
        "[" +
            ([
                for (name, fields) in sortBody -> 
                    let body = SortFieldListToJson fields 
                    "{\"" + name + "\":" + body + "}"
            ] |> String.concat ",")
        + "]"

[<AutoOpen>]
module ScriptFieldsDSL = 
    type ScriptFieldsBody = string * (ScriptQueryField list)
    
    let internal ScriptQueryToJson ((name, scriptBody): string * (ScriptQueryField list)) =
        "\"" + name + "\":{\"script\":{" + (ScriptQueryBodyToJSON scriptBody) + "}}"
        
    let internal ScriptFieldsBodyToJSON (fields: ScriptFieldsBody list) =
        [
            for field in fields ->
                ScriptQueryToJson field
        ] |> String.concat ","
        
[<AutoOpen>]
module AggsDSL =
    type AggsFieldsBody = 
        | NamedAgg of string * AggBody
        | MoreAggs of AggsFieldsBody list
        | FilterAgg of string * QueryBody * AggBody
    
    type AggWeightConfig = 
        | WeightField of string
        | Weight of string
    
    type AggParam = 
        | AggScript of (ScriptQueryField list)
        | AggValue of string
        | AggField of string 
        | AggWeight of AggWeightConfig
        | AggInterval of string
        | AggFormat of string
        | AggSize of int
        
    type AggBody = 
        | Avg of AggParam list
        | WeightedAvg of AggParam list
        | Max of AggParam list
        | Min of AggParam list
        | Sum of AggParam list
        | Stats of AggParam list
        | AggTerms of AggParam list
        | AggDateHistogram of AggParam list
        | AggFilter of QueryBody
    
    let internal AggParamsToJSON (aggParams:AggParam list) =
        [
            for param in aggParams ->
                match param with 
                | AggScript scriptBody -> "\"script\":{" + (ScriptQueryBodyToJSON scriptBody) + "}"
                | AggValue value -> "\"value\":\"" + value + "\""
                | AggField field -> "\"field\":\"" + field + "\""
                | AggWeight weightConfig -> 
                    match weightConfig with 
                    | WeightField field -> "\"weight\":{\"field\":\"" + field + "\"}"
                    | Weight weight-> "\"weight\":\"" + weight + "\""
                | AggInterval interval -> "\"interval\":\"" + interval + "\""
                | AggFormat format -> "\"format\":\"" + format + "\""
                | AggSize size -> "\"size\":" + size.ToString()
        ] |> String.concat ","
    
    let internal AggBodyToJSON ((name, body): (string * AggBody)) =
        "\"" + name + "\":{" +
            (
                let aggName, aggBody = 
                    match body with 
                    | Avg aggParams -> "avg", AggParamsToJSON aggParams
                    | WeightedAvg aggParams -> "weighted_avg", AggParamsToJSON aggParams
                    | Max aggParams -> "max", AggParamsToJSON aggParams
                    | Min aggParams -> "min", AggParamsToJSON aggParams
                    | Sum aggParams -> "sum", AggParamsToJSON aggParams
                    | Stats aggParams -> "stats", AggParamsToJSON aggParams
                    | AggTerms aggParams -> "terms", AggParamsToJSON aggParams
                    | AggDateHistogram aggParams -> "date_histogram", AggParamsToJSON aggParams
                "\"" + aggName + "\":{" + aggBody + "}"
            )
        + "}"
    
    let internal AggsBodyToJSON (fields: AggsFieldsBody list) = 
        [
            for field in fields ->
                match field with 
                | NamedAgg (name, agg) -> 
                    AggBodyToJSON (name, agg)
                | MoreAggs aggs ->
                    "\"aggs\":{" + (AggsBodyToJSON aggs) + "}"
                | FilterAgg (name, query, agg) ->
                    let queryBody = QueryBodyToJson query
                    let aggBody = AggBodyToJSON (name, agg)
                    "\"" + name + "\":{\"filter\":" + queryBody + ",\"aggs\":{" + aggBody + "}}"
        ] |> String.concat ","

[<AutoOpen>]
module SourceDSL =
    type Includes = string list
    type Excludes = string list
    
    type SourceBody =
    | Nothing
    | Only of string
    | List of string list
    | Pattern of Includes * Excludes
    
    let internal serializeList l =
        "[" + (l |> List.map (fun field -> "\"" + field + "\"") |> String.concat ",") + "]"
    
    let internal SourceBodyToJSON (body: SourceBody) : string =
        match body with
        | Nothing ->
            "false"
        | Only str ->
            "\"" + str + "\""
        | List l ->
            serializeList l
        | Pattern (i, e) ->
            "{\"includes\":" + serializeList i + ", \"excludes\":" + serializeList e + "}"
    

type ElasticDSL = 
    | Search of SearchBody list

and SearchBody = 
    | Query of QueryDSL.QueryBody
    | Sort of SortDSL.SortBody list
    | ScriptFields of ScriptFieldsBody list
    | Aggs of AggsFieldsBody list
    | From of int
    | Size of int
    | Source_ of SourceDSL.SourceBody
    
let ElasticDSLToJson (Search elasticBody:ElasticDSL) =
    "{" + 
        ([
            for searchBody in elasticBody ->  
                match searchBody with 
                | Query queryBody -> 
                    let body = QueryDSL.QueryBodyToJson queryBody
                    "\"query\":" + body
                | Sort sortBody ->
                    let body = SortDSL.SortBodyListToJson sortBody
                    "\"sort\":" + body
                | ScriptFields fields -> 
                    let body = ScriptFieldsDSL.ScriptFieldsBodyToJSON fields
                    "\"script_fields\":{" + body + "}"
                | Aggs fields -> 
                    let body = AggsDSL.AggsBodyToJSON fields
                    "\"aggs\":{" + body + "}"
                | From x -> 
                    "\"from\":" + x.ToString()
                | Size x -> 
                    "\"size\":" + x.ToString()
                | Source_ x ->
                    "\"_source\": " + SourceDSL.SourceBodyToJSON x  
        ] |> String.concat ",")
    + "}"
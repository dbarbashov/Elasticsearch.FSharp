module internal Elasticsearch.FSharp.DSL.Serialization.Aggs

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization

let AggParamsToJSON (aggParams:AggParam list) =
    [
        for param in aggParams ->
            match param with 
            | AggScript scriptBody -> "\"script\":{" + (Script.ScriptFieldsToJSON scriptBody) + "}"
            | AggValue value -> "\"value\":\"" + value + "\""
            | AggField field -> "\"field\":\"" + field + "\""
            | AggWeight weightConfig -> 
                match weightConfig with 
                | WeightField field -> "\"weight\":{\"field\":\"" + field + "\"}"
                | WeightValueField field -> "\"value\":{\"field\":\"" + field + "\"}"
                | Weight weight-> "\"weight\":\"" + weight + "\""
            | AggInterval interval -> "\"interval\":\"" + interval + "\""
            | AggFormat format -> "\"format\":\"" + format + "\""
            | AggSize size -> "\"size\":" + size.ToString()
    ] |> String.concat ","

let AggBodyToJSON ((name, body): (string * AggBody)) =
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

let rec AggsBodyToJSON (fields: AggsFieldsBody list) = 
    [
        for field in fields ->
            match field with 
            | NamedAgg (name, agg) -> 
                AggBodyToJSON (name, agg)
            | MoreAggs aggs ->
                "\"aggs\":{" + (AggsBodyToJSON aggs) + "}"
            | FilterAgg (name, query, agg) ->
                let queryBody = Query.QueryBodyToJson query
                let aggBody = AggBodyToJSON (name, agg)
                "\"" + name + "\":{\"filter\":" + queryBody + ",\"aggs\":{" + aggBody + "}}"
    ] |> String.concat ","

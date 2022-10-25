module internal rec Elasticsearch.FSharp.DSL.Serialization.Aggs

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization
open Elasticsearch.FSharp.Utility

type AggWeightConfig with
    member x.ToJson() =
        match x with 
        | WeightField field ->
            Json.makeKeyValue "weight" (Json.makeObject [
                Json.makeKeyValue "field" (Json.quoteString field)
            ])
        | WeightValueField field ->
            Json.makeKeyValue "value" (Json.makeObject [
                Json.makeKeyValue "field" (Json.quoteString field)
            ])
        | Weight weight->
            Json.makeKeyValue "weight" (Json.quoteString weight)

type AggParam with
    member x.ToJson() =
        match x with
        | AggScript scriptBody ->
            Json.makeKeyValue "script" (ScriptField.scriptFieldsToJson scriptBody)
        | AggValue value ->
            Json.makeKeyValue "value" (Json.quoteString value)
        | AggField field ->
            Json.makeKeyValue "field" (Json.quoteString field)
        | AggWeight weightConfig ->
            weightConfig.ToJson()
        | AggInterval interval ->
            Json.makeKeyValue "interval" (Json.quoteString interval)
        | AggFormat format ->
            Json.makeKeyValue "format" (Json.quoteString format)
        | AggSize size -> 
            Json.makeKeyValue "size" (size.ToString())
        

let aggParamsToJson (aggParams:AggParam list) =
    Json.makeObject [
        for param in aggParams ->
            param.ToJson()
    ]

type AggBody with
    member x.ToJson() =
        match x with 
        | Avg aggParams ->
            Json.makeKeyValue "avg" (aggParamsToJson aggParams)
        | WeightedAvg aggParams ->
            Json.makeKeyValue "weighted_avg" (aggParamsToJson aggParams)
        | Max aggParams ->
            Json.makeKeyValue "max" (aggParamsToJson aggParams)
        | Min aggParams ->
            Json.makeKeyValue "min" (aggParamsToJson aggParams)
        | Sum aggParams ->
            Json.makeKeyValue "sum" (aggParamsToJson aggParams)
        | Stats aggParams ->
            Json.makeKeyValue "stats" (aggParamsToJson aggParams)
        | AggTerms aggParams ->
            Json.makeKeyValue "terms" (aggParamsToJson aggParams)
        | AggDateHistogram aggParams ->
            Json.makeKeyValue "date_histogram" (aggParamsToJson aggParams)
        | ValueCount aggParams ->
            Json.makeKeyValue "value_count" (aggParamsToJson aggParams)

let complexAggsBodyToJson (complexAggs: AggsFieldsBody list) =
    Json.makeKeyValue "aggs" (Json.makeObject [
        for complexAgg in complexAggs ->
            complexAgg.ToJson()
    ])

let aggBodyToJson ((name, body): (string * AggBody)) (complexAggs: AggsFieldsBody list) =
    Json.makeKeyValue name (Json.makeObject [
        body.ToJson()
        if List.tryHead complexAggs <> None then
            complexAggsBodyToJson complexAggs
    ])

type AggsFieldsBody with
    member x.ToJson() =
        match x with
        | NamedAgg (name, agg) -> 
            aggBodyToJson (name, agg) []
        | FilterAgg (name, query, agg) ->
            Json.makeKeyValue name (Json.makeObject [
                Json.makeKeyValue "filter" (Query.queryBodyToJson query)
                Json.makeKeyValue "aggs" (Json.makeObject [
                    aggBodyToJson (name, agg) []
                ])
            ])
        | NamedComplexAgg (name, agg, complexAggs) ->
            aggBodyToJson (name, agg) complexAggs
        | FilterComplexAgg (name, query, agg, complexAggs) ->
            Json.makeKeyValue name (Json.makeObject [
                Json.makeKeyValue "filter" (Query.queryBodyToJson query)
                Json.makeKeyValue "aggs" (Json.makeObject [
                    aggBodyToJson (name, agg) complexAggs
                ])
            ])

let rec aggsBodyToJson (fields: AggsFieldsBody list) =
    Json.makeObject [
        for field in fields ->
            field.ToJson()
    ]

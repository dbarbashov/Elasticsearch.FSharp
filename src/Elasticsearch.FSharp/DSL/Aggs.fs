namespace Elasticsearch.FSharp.DSL

type AggsFieldsBody = 
    | NamedAgg of string * AggBody
    | MoreAggs of AggsFieldsBody list
//    | FilterAgg of string * QueryBody * AggBody

and AggWeightConfig = 
    | WeightField of string
    | Weight of string

and AggParam = 
    | AggScript of (ScriptField list)
    | AggValue of string
    | AggField of string 
    | AggWeight of AggWeightConfig
    | AggInterval of string
    | AggFormat of string
    | AggSize of int
    
and AggBody = 
    | Avg of AggParam list
    | WeightedAvg of AggParam list
    | Max of AggParam list
    | Min of AggParam list
    | Sum of AggParam list
    | Stats of AggParam list
    | AggTerms of AggParam list
    | AggDateHistogram of AggParam list
//    | AggFilter of QueryBody

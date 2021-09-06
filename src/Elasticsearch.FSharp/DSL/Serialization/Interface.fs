namespace Elasticsearch.FSharp.DSL.Serialization

open Elasticsearch.FSharp.DSL.Serialization.Search

[<AutoOpen>]
module Interface = 
    let toJson = ElasticDSLToJson
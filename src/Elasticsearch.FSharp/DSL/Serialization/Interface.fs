namespace Elasticsearch.FSharp.DSL.Serialization

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization.Search

[<AutoOpen>]
module Interface = 
    let toJson (x: ElasticDSL) = x.ToJson()
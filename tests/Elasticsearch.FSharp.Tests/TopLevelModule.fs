module Elasticsearch.FSharp.Tests

open Elasticsearch.FSharp.Module
open Nest
open Xunit

[<Fact>]
let ``Create match all query``() =
    let _ = elastic<unit>() {
        query (
            match_all { () }
        )
    }
    ()

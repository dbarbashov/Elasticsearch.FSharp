module Elasticsearch.FSharp.Tests

open System
open Elasticsearch.FSharp.Module
open Nest
open Xunit
open Elasticsearch.FSharp.Tests.Helper

[<Fact>]
let ``Create match all query``() =
    let actual = elastic<unit>() {
        query (
            match_all()
        )
        size 10000
    }
    let expected =
        SearchDescriptor<unit>()
            .Query(fun q -> q.MatchAll())
            .Size(Nullable 10000)
            
    Assert.EqualQuery(expected, actual)
    
[<Fact>]
let ``Create bool query``() =
    let actual = elastic<unit>() {
        query (
            bool {
                must [
                    match_all()
                ]
                should [
                    match_all()
                ]
                must_not [
                    match_all()
                ]
                minimum_should_match "2.0"
            }
        )
    }
    let expected =
        SearchDescriptor<unit>()
            .Query(fun q -> q.Bool(fun b ->
                                    b.Must(fun (q:QueryContainerDescriptor<unit>) -> q.MatchAll())
                                     .Should(fun (q:QueryContainerDescriptor<unit>) -> q.MatchAll())
                                     .MustNot(fun (q:QueryContainerDescriptor<unit>) -> q.MatchAll())
                                     .MinimumShouldMatch(MinimumShouldMatch("2.0"))
                                     :> IBoolQuery
                                   ))
    Assert.EqualQuery(expected, actual)
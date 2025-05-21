module Elasticsearch.FSharp.Tests.SearchBody.TrackTotalHitsTests

open NUnit.Framework
open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization
open Elasticsearch.FSharp.Utility

[<Test>]
let ``"track_total_hits" true serializes correctly``() =
    let query =
        Search [
            TrackTotalHits true
            Query MatchAll
        ]
    let expected = """{"track_total_hits":true,"query":{"match_all":{}}}"""
    let actual = toJson query
    Assert.AreEqual(expected, actual)

[<Test>]
let ``"track_total_hits" false serializes correctly``() =
    let query =
        Search [
            TrackTotalHits false
            Query MatchAll
        ]
    let expected = """{"track_total_hits":false,"query":{"match_all":{}}}"""
    let actual = toJson query
    Assert.AreEqual(expected, actual)

[<Test>]
let ``"track_total_hits" true as only search body element serializes correctly``() =
    let query =
        Search [
            TrackTotalHits true
        ]
    let expected = """{"track_total_hits":true}"""
    let actual = toJson query
    Assert.AreEqual(expected, actual)

[<Test>]
let ``"track_total_hits" false as only search body element serializes correctly``() =
    let query =
        Search [
            TrackTotalHits false
        ]
    let expected = """{"track_total_hits":false}"""
    let actual = toJson query
    Assert.AreEqual(expected, actual)

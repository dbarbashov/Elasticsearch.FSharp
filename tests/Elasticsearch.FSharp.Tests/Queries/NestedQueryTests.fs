module Elasticsearch.FSharp.Tests.Queries.NestedQueryTests

open NUnit.Framework
open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization
open Elasticsearch.FSharp.Utility
open Elasticsearch.FSharp.Tests.Helpers

[<Test>]
let ``"nested" query basic serializes correctly``() =
    let query =
        Search [
            Query (
                Nested [
                    NestedQueryField.Path "obj1"
                    NestedQueryField.QueryBody (
                        Match ("obj1.name", [MatchQuery "blue"])
                    )
                ]
            )
        ]
    let expected = """{"query":{"nested":{"path":"obj1","query":{"match":{"obj1.name":{"query":"blue"}}}}}}"""
    let actual = toJson query
    Assert.AreEqual(Helpers.removeWhitespace expected, Helpers.removeWhitespace actual)

[<Test>]
let ``"nested" query with score_mode serializes correctly``() =
    let query =
        Search [
            Query (
                Nested [
                    NestedQueryField.Path "obj1"
                    NestedQueryField.QueryBody (MatchAll)
                    NestedQueryField.ScoreMode ScoreModeOption.ScoreModeMax
                ]
            )
        ]
    let expected = """{"query":{"nested":{"path":"obj1","query":{"match_all":{}},"score_mode":"max"}}}"""
    let actual = toJson query
    Assert.AreEqual(Helpers.removeWhitespace expected, Helpers.removeWhitespace actual)

[<Test>]
let ``"nested" query with ignore_unmapped serializes correctly``() =
    let query =
        Search [
            Query (
                Nested [
                    NestedQueryField.Path "obj1"
                    NestedQueryField.QueryBody (MatchAll)
                    NestedQueryField.IgnoreUnmapped true
                ]
            )
        ]
    let expected = """{"query":{"nested":{"path":"obj1","query":{"match_all":{}},"ignore_unmapped":true}}}"""
    let actual = toJson query
    Assert.AreEqual(Helpers.removeWhitespace expected, Helpers.removeWhitespace actual)

[<Test>]
let ``"nested" query with all options serializes correctly``() =
    let query =
        Search [
            Query (
                Nested [
                    NestedQueryField.Path "obj1.child"
                    NestedQueryField.QueryBody (
                        Bool [
                            Must [ Term ("obj1.child.field", [ExactValue "value"]) ]
                        ]
                    )
                    NestedQueryField.ScoreMode ScoreModeOption.ScoreModeSum
                    NestedQueryField.IgnoreUnmapped false
                ]
            )
        ]
    let expected = """{"query":{"nested":{"path":"obj1.child","query":{"bool":{"must":[{"term":{"obj1.child.field":{"value":"value"}}}]}},"score_mode":"sum","ignore_unmapped":false}}}"""
    let actual = toJson query
    Assert.AreEqual(Helpers.removeWhitespace expected, Helpers.removeWhitespace actual)

[<Test>]
let ``"nested" query multi-level serializes correctly``() =
    let query =
        Search [
            Query (
                Nested [
                    NestedQueryField.Path "driver"
                    NestedQueryField.QueryBody (
                        Nested [
                            NestedQueryField.Path "driver.vehicle"
                            NestedQueryField.QueryBody (
                                Bool [
                                    Must [
                                        Match ("driver.vehicle.make", [MatchQuery "Powell Motors"])
                                        Match ("driver.vehicle.model", [MatchQuery "Canyonero"])
                                    ]
                                ]
                            )
                        ]
                    )
                ]
            )
        ]
    let expected = """{"query":{"nested":{"path":"driver","query":{"nested":{"path":"driver.vehicle","query":{"bool":{"must":[{"match":{"driver.vehicle.make":{"query":"Powell Motors"}}},{"match":{"driver.vehicle.model":{"query":"Canyonero"}}}]}}}}}}}"""
    let actual = toJson query
    Assert.AreEqual(Helpers.removeWhitespace expected, Helpers.removeWhitespace actual)

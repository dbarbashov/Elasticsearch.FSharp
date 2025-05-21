module Elasticsearch.FSharp.Tests.Queries.RangeQueryTests

open NUnit.Framework
open FsCheck.NUnit
open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization
open Elasticsearch.FSharp.Utility
open Elasticsearch.FSharp.Tests.Helpers

[<Property>]
let ``"range" with gte serializes correctly``(fieldName, fieldValue) =
    let query =
        Search [
            Query (
                Range (fieldName, [Gte fieldValue])
            )
        ]
    let expected = sprintf """{"query":{"range":{"%s":{"gte":"%s"}}}}"""
                       (Json.escapeString fieldName) (Json.escapeString fieldValue)
    let actual = toJson query
    Assert.AreEqual(expected, actual)

[<Property>]
let ``"range" with gt serializes correctly``(fieldName, fieldValue) =
    let query = Search [ Query (Range (fieldName, [Gt fieldValue])) ]
    let expected = sprintf """{"query":{"range":{"%s":{"gt":"%s"}}}}""" (Json.escapeString fieldName) (Json.escapeString fieldValue)
    Assert.AreEqual(expected, toJson query)

[<Property>]
let ``"range" with lte serializes correctly``(fieldName, fieldValue) =
    let query = Search [ Query (Range (fieldName, [Lte fieldValue])) ]
    let expected = sprintf """{"query":{"range":{"%s":{"lte":"%s"}}}}""" (Json.escapeString fieldName) (Json.escapeString fieldValue)
    Assert.AreEqual(expected, toJson query)

[<Property>]
let ``"range" with lt serializes correctly``(fieldName, fieldValue) =
    let query = Search [ Query (Range (fieldName, [Lt fieldValue])) ]
    let expected = sprintf """{"query":{"range":{"%s":{"lt":"%s"}}}}""" (Json.escapeString fieldName) (Json.escapeString fieldValue)
    Assert.AreEqual(expected, toJson query)

[<Property>]
let ``"range" with time_zone serializes correctly``(fieldName, timeZone) =
    let query = Search [ Query (Range (fieldName, [Gte "now-1h"; RangeTimeZone timeZone])) ]
    let expected = sprintf """{"query":{"range":{"%s":{"gte":"now-1h","time_zone":"%s"}}}}""" (Json.escapeString fieldName) (Json.escapeString timeZone)
    Assert.AreEqual(expected, toJson query)

[<Test>]
let ``"range" with multiple conditions serializes correctly``() =
    let fieldName = "date"
    let query =
        Search [
            Query (
                Range (fieldName, [
                    Gte "2020-01-01"
                    Lte "2020-12-31"
                    RangeTimeZone "+01:00"
                ])
            )
        ]
    let expected = """{"query":{"range":{"date":{"gte":"2020-01-01","lte":"2020-12-31","time_zone":"+01:00"}}}}"""
    Assert.AreEqual(Helpers.removeWhitespace expected, Helpers.removeWhitespace (toJson query))

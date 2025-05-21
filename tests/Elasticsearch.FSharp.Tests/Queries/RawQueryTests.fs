module Elasticsearch.FSharp.Tests.Queries.RawQueryTests

open NUnit.Framework
open FsCheck.NUnit
open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.DSL.Serialization
open Elasticsearch.FSharp.Utility
open Elasticsearch.FSharp.Tests.Helpers

[<Property(MaxTest=10000)>]
let ``"raw" serialization works correctly``(rawQuery) =
    let query =
        Search [
            Query(
                QueryBody.Raw rawQuery
            )
        ]
    let expected = sprintf """{"query":{%s}}""" rawQuery
    let actual = toJson query
    Assert.AreEqual(expected, actual)

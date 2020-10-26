module Elasticsearch.FSharp.Tests.Helper

open System.IO
open Nest
open Xunit

let elasticClient = ElasticClient()
let serializeToString (searchRequest: ISearchRequest) =
    use outputStream = new MemoryStream()
    elasticClient.SourceSerializer.Serialize(searchRequest, outputStream)
    outputStream.Seek(0L, SeekOrigin.Begin) |> ignore
    use sr = new StreamReader(outputStream)
    sr.ReadToEnd()

type Assert with
    static member EqualQuery(expected: ISearchRequest, actual: ISearchRequest) =
        Assert.Equal(serializeToString expected, serializeToString actual)

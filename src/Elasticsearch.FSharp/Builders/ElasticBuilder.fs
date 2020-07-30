namespace Elasticsearch.FSharp.Builders

open System
open Nest

type ElasticBuilder<'T when 'T: not struct>() =
    
    let search = SearchDescriptor<'T>()
    
    [<CustomOperation("size")>]
    member x.Size(a, size: int) =
        search.Size(Nullable size)
        
    [<CustomOperation("query")>]
    member x.Query(a, q: Func<QueryContainerDescriptor<'T>, QueryContainer>) =
        search.Query(q)
        
    [<CustomOperation("aggs")>]
    member x.Aggs(a, aggs) =
        ()
        
    [<CustomOperation("sort")>]
    member x.Sort(a, sort) =
        ()
        
    member x.Yield(a) =
        search
        
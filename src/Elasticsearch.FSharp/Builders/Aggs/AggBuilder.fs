namespace Elasticsearch.FSharp.Builders.Aggs

type AggBuilder(name: string) =
    
    [<CustomOperation("body")>]
    member x.Body(a, t) =
        a
        
    [<CustomOperation("aggs")>]
    member x.Aggs(a, b) =
        a
        
    [<CustomOperation("meta")>]
    member x.Meta(a, b) =
        a
    
    member x.Yield(a) =
        a
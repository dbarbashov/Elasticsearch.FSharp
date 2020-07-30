namespace Elasticsearch.FSharp.Builders.Query

open System
open Nest

type MatchAllBuilder() =
    
    member x.Zero() =
        Func<_,_>(
            fun (q: QueryContainerDescriptor<'T>) ->
                q.MatchAll()
        )
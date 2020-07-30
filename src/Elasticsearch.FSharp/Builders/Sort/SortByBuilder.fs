namespace Elasticsearch.FSharp.Builders.Sort

type SortByBuilder() =
    
    [<CustomOperation("field")>]
    member x.Field(a, b) =
        a
        
    [<CustomOperation("order")>]
    member x.Order(a, b) =
        a

    [<CustomOperation("mode")>]
    member x.Mode(a, b) =
        a

    member x.Yield(a) = a

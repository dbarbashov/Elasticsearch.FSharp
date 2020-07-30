namespace Elasticsearch.FSharp.Builders.Aggs

type TermsBuilder() =
    
    [<CustomOperation("field")>]
    member x.Field(a, f) =
        a
    
    [<CustomOperation("size")>]    
    member x.Size(a, s) =
        a

    [<CustomOperation("show_term_doc_count_error")>]
    member x.ShowTermDocCountError(a, b:bool) =
        a
    
    member x.Yield(a) =
        a


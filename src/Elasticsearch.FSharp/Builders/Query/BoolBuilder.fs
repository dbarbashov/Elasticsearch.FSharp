namespace Elasticsearch.FSharp.Builders.Query

open System
open Nest

type BoolBuilder<'T when 'T: not struct>() =
    
    let calls = ResizeArray()
    
    [<CustomOperation("must")>]
    member x.Must(a, b: Func<QueryContainerDescriptor<'T>, QueryContainer> seq) =
        calls.Add(fun (x:BoolQueryDescriptor<'T>) -> x.Must b)
        a
        
    [<CustomOperation("must_not")>]
    member x.MustNot(a, b: Func<QueryContainerDescriptor<'T>, QueryContainer> seq) =
        calls.Add(fun (x:BoolQueryDescriptor<'T>) -> x.MustNot b)
        a

    [<CustomOperation("should")>]
    member x.Should(a, b: Func<QueryContainerDescriptor<'T>, QueryContainer> seq) =
        calls.Add(fun x -> x.Should b)
        a
        
    [<CustomOperation("minimum_should_match")>]
    member x.MinimumShouldMatch(a, v: string) =
        calls.Add(fun x -> x.MinimumShouldMatch(MinimumShouldMatch.op_Implicit v))
        a
    
    member x.Yield(_) =
        Func<_,_>(
            fun (q: QueryContainerDescriptor<'T>) ->
                q.Bool(
                    fun boolQueryDescriptor ->
                        Seq.fold (fun x f -> f x) boolQueryDescriptor calls :> IBoolQuery
                )
        )

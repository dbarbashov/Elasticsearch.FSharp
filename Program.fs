module Program

open Elasticsearch.FSharp.DSL
open Elasticsearch.FSharp.Mapping

type SubSubType = {
    [<ElasticField("keyword")>]
    SomeFieldAgain: string
}

type SubType = {
    [<ElasticField("text")>]
    SomeField: string
    
    [<ElasticField(useProperties=true)>]
    SomeField2: SubSubType
}

[<ElasticType("demo_type")>]
type DemoType = {
    [<ElasticField("keyword")>]
    KeywordField: string
    
    [<ElasticField("text", analyzer="russian")>]
    TextField: string
    
    [<ElasticField("date", format="yyyy-MM-dd")>]
    DateField: string
    
    [<ElasticField(enabled=false)>]
    DisabledField: string
    
    [<ElasticField(useProperties=true)>]
    TypeField: SubType
}

[<EntryPoint>]
let main _ = 

    let elasticQuery customerId from to' =
        Search [
            Size 1
            Query (
                Exists "customer_id"
//                MultiMatch [
//                    MultiMatchQuery "test"
//                    Fields ["firstname"; "lastname"; "middlename"; "phone_number"; "agreement_no"; "text"; "dialog_id"]
//                ]
            )
            Aggs [
                NamedAgg ("dialog_ids", AggTerms [AggField "dialog_id"; AggSize 1000])
                FilterAgg ("dialog_ids", TypeEquals "message", AggTerms [AggField "dialog_id"; AggSize 1000])
            ]
        ]
        
    let json = ElasticDSLToJson (elasticQuery "nspk" "20000" "300000")     
    System.Console.WriteLine(json)

//    let t1 = DateTime.Now.Ticks
//    (GenerateElasticMappings typeof<DemoType>) |> ignore
//    printfn "%A" (double(DateTime.Now.Ticks - t1) / double(TimeSpan.TicksPerMillisecond))
//    
//    let iters = 10000
//    let t1 = DateTime.Now.Ticks
//    for i in 1..iters do 
//        (GenerateElasticMappings typeof<DemoType>) |> ignore
//    printfn "%A" (double(DateTime.Now.Ticks - t1) / double(TimeSpan.TicksPerMillisecond) / double(iters))

//    printfn "%A" (GenerateElasticMappings typeof<DemoType>)

    0

module Elasticsearch.FSharp.Tests.Mapping

open NUnit.Framework

open System.Text.RegularExpressions
open Elasticsearch.FSharp.Mapping
open Elasticsearch.FSharp.Mapping.ElasticMappingDSL

[<ElasticType("custom_entity_name")>]
type TestEntity = {
    [<ElasticField("long")>]
    id: int64
    
    [<ElasticField("text")>]
    [<ElasticSubField("raw", fieldType = "keyword")>]
    [<ElasticSubField("en", fieldType = "text", analyzer = "english")>]
    [<ElasticSubField("ru", fieldType = "text", analyzer = "russian")>]
    title: string
}
    

[<Test>]
let ``Type serializes correctly``() =
    let typeMapping = GenerateElasticMappings typeof<TestEntity>
    let indexMapping =
            ElasticMapping [
                Mappings [
                    typeMapping
                ]
            ]
    let mappingJson = ElasticMappingToJSON indexMapping
    let expected =
        Regex.Replace(
            """{
                "mappings": {
                    "custom_entity_name": {
                        "properties": {
                            "id": {
                                "type": "long"
                            },
                            "title": {
                                "fields": {
                                    "raw": { "type":"keyword" },
                                    "en": { "type":"text", "analyzer":"english" },
                                    "ru": { "type":"text", "analyzer":"russian" }
                                },
                                "type": "text"
                            }
                        }
                    }
                }
            }""",
            "\s*",
            ""
        )
    let actual = mappingJson
    printf "%s" mappingJson
    Assert.AreEqual(expected, actual)
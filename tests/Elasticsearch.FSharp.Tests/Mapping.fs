module Elasticsearch.FSharp.Tests.Mapping

open NUnit.Framework

open System.Text.RegularExpressions
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
    
[<ElasticType("message")>]
type Elastic_Message = {
    [<ElasticField("keyword")>]
    [<ElasticSubField("raw", fieldType="keyword")>]
    [<ElasticSubField("ru", fieldType="text", analyzer="ru")>]
    [<ElasticSubField("en", fieldType="text", analyzer="en")>]
    id: string
    
    [<ElasticField(useProperties=true, maxDepth=1)>]
    reply_to_message: Elastic_Message option
}

[<Test>]
let ``Recursive type serializes correctly``() =
    let typeMapping = GenerateElasticMappings typeof<Elastic_Message>
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
                    "message": {
                        "properties": {
                            "id": {
                                "fields": {
                                    "raw": {
                                        "type": "keyword"
                                    },
                                    "ru": {
                                        "type": "text",
                                        "analyzer": "ru"
                                    },
                                    "en": {
                                        "type": "text",
                                        "analyzer": "en"
                                    }
                                },
                                "type": "keyword"
                            },
                            "reply_to_message": {
                                "properties": {
                                    "id": {
                                        "fields": {
                                            "raw": {
                                                "type": "keyword"
                                            },
                                            "ru": {
                                                "type": "text",
                                                "analyzer": "ru"
                                            },
                                            "en": {
                                                "type": "text",
                                                "analyzer": "en"
                                            }
                                        },
                                        "type": "keyword"
                                    }
                                }
                            }
                        }
                    }
                }
            }""",
            "\s*",
            "")
    let actual = mappingJson
    printf "%s" mappingJson
    Assert.AreEqual(expected, actual)
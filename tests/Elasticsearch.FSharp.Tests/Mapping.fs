module Elasticsearch.FSharp.Tests.Mapping

open NUnit.Framework

open System.Text.RegularExpressions
open Elasticsearch.FSharp.Mapping.Attributes
open Elasticsearch.FSharp.Mapping.Json

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
    let mapping = generateElasticMapping typeof<TestEntity>
    let mappingJson = mapping.ToJson()
    let expected =
        Regex.Replace(
            """{
                "mappings": {
                    "_doc": {
                        "properties": {
                            "id": {
                                "type": "long"
                            },
                            "title": {
                                "type": "text",
                                "fields": {
                                    "raw": { "type":"keyword" },
                                    "en": { "type":"text", "analyzer":"english" },
                                    "ru": { "type":"text", "analyzer":"russian" }
                                }
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
    let mapping = generateElasticMapping typeof<Elastic_Message>
    let mappingJson = mapping.ToJson()
    let expected =
        Regex.Replace(
            """{
                "mappings": {
                    "_doc": {
                        "properties": {
                            "id": {
                                "type": "keyword",
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
                                }
                            },
                            "reply_to_message": {
                                "properties": {
                                    "id": {
                                        "type": "keyword",
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
                                        }
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
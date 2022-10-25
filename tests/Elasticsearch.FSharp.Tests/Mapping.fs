module Elasticsearch.FSharp.Tests.Mapping

open System.Collections.Generic
open Elasticsearch.FSharp.Mapping.DSL
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
    [<ElasticSubField("integer", fieldType = "integer", ignoreMalformed=true)>]
    [<ElasticSubField("en", fieldType = "text", analyzer = "english")>]
    [<ElasticSubField("ru", fieldType = "text", analyzer = "russian")>]
    title: string
}

[<Test>]
let ``Type serializes correctly``() =
    let mapping = generateElasticMapping typeof<TestEntity>
    let mappingJson = mapping.ToJson()
    let expected =
        Helpers.removeWhitespace
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
                                    "integer": { "ignore_malformed":true, "type":"integer" },
                                    "en": { "type":"text", "analyzer":"english" },
                                    "ru": { "type":"text", "analyzer":"russian" }
                                }
                            }
                        }
                    }
                }
            }"""
    let actual = mappingJson
    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``Type serializes correctly with excluded type name``() =
    let mapping = generateElasticMapping typeof<TestEntity>
    let mappingJson = mapping.ToJson(includeTypeName=false)
    let expected =
        Helpers.removeWhitespace
            """{
                "mappings": {
                    "properties": {
                        "id": {
                            "type": "long"
                        },
                        "title": {
                            "type": "text",
                            "fields": {
                                "raw": { "type":"keyword" },
                                "integer": { "ignore_malformed":true, "type":"integer" },
                                "en": { "type":"text", "analyzer":"english" },
                                "ru": { "type":"text", "analyzer":"russian" }
                            }
                        }
                    }
                }
            }"""
    let actual = mappingJson
    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``Type serializes correctly with settings``() =
    let mapping = generateElasticMapping typeof<TestEntity>
    let mapping =
        { mapping with 
            Settings =
                [
                    "number_of_shards", MappingSetting.Setting "5"
                    "number_of_replicas", MappingSetting.Setting "2"
                ]
                |> List.map (fun (a, b) -> KeyValuePair(a, b))
                |> Dictionary
                |> Some
        }
    let mappingJson = mapping.ToJson(includeTypeName=false)
    let expected =
        Helpers.removeWhitespace
            """{
                "settings": {
                    "number_of_shards": "5",
                    "number_of_replicas": "2"
                },
                "mappings": {
                    "properties": {
                        "id": {
                            "type": "long"
                        },
                        "title": {
                            "type": "text",
                            "fields": {
                                "raw": { "type":"keyword" },
                                "integer": { "ignore_malformed":true, "type":"integer" },
                                "en": { "type":"text", "analyzer":"english" },
                                "ru": { "type":"text", "analyzer":"russian" }
                            }
                        }
                    }
                }
            }"""
    let actual = mappingJson
    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``Type serializes correctly to put mappings json``() =
    let mapping = generateElasticMapping typeof<TestEntity>
    let mappingJson = mapping.ToPutMappingsJson()
    let expected =
        [|
            """{"properties":{"id": { "type": "long" }}}"""
            """{"properties":{"title": { "type": "text", "fields": { "raw": { "type":"keyword" }, "integer": { "ignore_malformed":true, "type":"integer" }, "en": { "type":"text", "analyzer":"english" }, "ru": { "type":"text", "analyzer":"russian" } } }}}"""
        |]
        |> Array.map Helpers.removeWhitespace
    let actual = mappingJson
    CollectionAssert.AreEquivalent(expected, actual)
    
[<ElasticType("message")>]
type Elastic_Message = {
    [<ElasticField("keyword")>]
    [<ElasticSubField("raw", fieldType="keyword")>]
    [<ElasticSubField("integer", fieldType = "integer", ignoreMalformed = true)>]
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
        Helpers.removeWhitespace
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
                                    "integer": {
                                        "ignore_malformed":true,
                                        "type":"integer"
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
                                            "integer": {
                                                "ignore_malformed":true,
                                                "type":"integer"
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
            }"""
    let actual = mappingJson
    printf "%s" mappingJson
    Assert.AreEqual(expected, actual)
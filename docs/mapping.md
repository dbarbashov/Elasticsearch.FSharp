# Mapping Generation

The library provides utilities to generate Elasticsearch mapping definitions from F# types decorated with specific attributes. This is useful for setting up your indices with the correct field types and properties before indexing data.

## Defining an Entity

Use attributes from the `Elasticsearch.FSharp.Mapping.Attributes` namespace to define how your F# types map to Elasticsearch fields.

```fsharp
open Elasticsearch.FSharp.Mapping.Attributes

[<ElasticType("custom_entity_name")>] // Optional: sets the Elasticsearch type name.
                                     // If includeTypeName=true in ToJson, this is used. Otherwise, _doc or no type.
type TestEntity = {
    [<ElasticField("long")>] // Specifies the Elasticsearch field type
    id: int64

    [<ElasticField("text")>]
    [<ElasticSubField("raw", fieldType = "keyword")>] // Defines a multi-field for 'title'
    [<ElasticSubField("en", fieldType = "text", analyzer = "english")>]
    title: string

    [<ElasticField("text", name = "overridden_field_name")>] // 'name' overrides the F# property name in JSON
    originalPropertyName: string

    [<ElasticField("keyword", ignoreAbove = 256u)>]
    tag: string

    [<ElasticField("integer", ignoreMalformed = true)>]
    count: int
}
```

## Generating Mapping JSON

Use `generateElasticMapping` from `Elasticsearch.FSharp.Mapping.DSL` and `ToJson()` or `ToPutMappingsJson()` from `Elasticsearch.FSharp.Mapping.Json`.

```fsharp
open Elasticsearch.FSharp.Mapping.DSL
open Elasticsearch.FSharp.Mapping.Json

// Generate the mapping structure from the F# type
let mapping = generateElasticMapping typeof<TestEntity>

// Generates JSON for creating an index with mapping (includes "mappings" and type name wrapper by default)
let createIndexJson = mapping.ToJson()

// Generates JSON for creating an index, excluding the type name wrapper under "mappings"
let createIndexJsonNoType = mapping.ToJson(includeTypeName=false)

// To get JSON for the PUT mapping API (just the "properties" part, one string per top-level property)
let putMappingJsonParts = mapping.ToPutMappingsJson()
```

## Example JSON Output (`mapping.ToJson(includeTypeName=false)`)

For the `TestEntity` above, `mapping.ToJson(includeTypeName=false)` would produce something like:

```json
{
  "mappings": {
    "properties": {
      "id": { "type": "long" },
      "title": {
        "type": "text",
        "fields": {
          "raw": { "type": "keyword" },
          "en": { "type": "text", "analyzer": "english" }
        }
      },
      "overridden_field_name": { "type": "text" },
      "tag": { "type": "keyword", "ignore_above": 256 },
      "count": { "type": "integer", "ignore_malformed": true }
    }
  }
}
```
If `mapping.ToJson()` (or `mapping.ToJson(includeTypeName=true)`) is used, and `ElasticType` attribute is present, its value (`custom_entity_name`) would wrap the `properties` object. If `ElasticType` is absent, `_doc` would be used as the wrapper.

## Attributes

- `[<ElasticType("type_name")>]`: (Optional) Applied to the record/class type. Specifies the Elasticsearch document type name. This is used if `ToJson(includeTypeName=true)` is called.
- `[<ElasticField("field_type", ...)>]`: Applied to record fields/properties.
    - `fieldType: string`: The Elasticsearch data type (e.g., "text", "keyword", "long", "date", "object", "nested").
    - `name: string`: (Optional) Overrides the F# field name in the generated JSON mapping.
    - `ignoreAbove: uint32`: (Optional) For `keyword` fields, sets `ignore_above`.
    - `ignoreMalformed: bool`: (Optional) For numeric, date, geo fields, sets `ignore_malformed`.
    - `useProperties: bool`: (Optional) For object types, indicates that the properties of the complex type should be mapped. Defaults to `true` if the field is a record/class type.
    - `maxDepth: int`: (Optional) For recursive types, limits the depth of property mapping to prevent infinite recursion.
- `[<ElasticSubField("sub_field_name", ...)>]`: Applied to record fields/properties to define multi-fields (sub-fields).
    - `subFieldName: string`: The name of the sub-field (e.g., "raw", "english").
    - `fieldType: string`: The Elasticsearch type for this sub-field.
    - `analyzer: string`: (Optional) Analyzer for text sub-fields.
    - `ignoreMalformed: bool`: (Optional) For sub-fields.

## Including Index Settings

You can add index-level settings (like `number_of_shards` or `analysis` configurations) to the mapping programmatically:

```fsharp
open System.Collections.Generic
open Elasticsearch.FSharp.Utility // For Json.makeObject, etc. if building complex settings

let mappingWithSettings =
    { mapping with // Assuming 'mapping' is from generateElasticMapping
        Settings =
            [
                "number_of_shards", MappingSetting.Setting "5" // Simple string setting
                "number_of_replicas", MappingSetting.Setting "1"
                "analysis", MappingSetting.AnalyzerBlock ( // For complex JSON blocks like analyzers
                    Json.makeObject [
                        Json.makeKeyValue "analyzer" (Json.makeObject [
                            Json.makeKeyValue "my_custom_analyzer" (Json.makeObject [
                                Json.makeKeyValue "tokenizer" (Json.quoteString "standard")
                                Json.makeKeyValue "filter" (Json.makeArray [Json.quoteString "lowercase"])
                            ])
                        ])
                    ]
                )
            ]
            |> List.map (fun (k, v) -> KeyValuePair(k, v))
            |> Dictionary
            |> Some
    }
let mappingJsonWithSettings = mappingWithSettings.ToJson(includeTypeName=false)
```

JSON Output with settings (`mappingJsonWithSettings`):
```json
{
  "settings": {
    "number_of_shards": "5",
    "number_of_replicas": "1",
    "analysis": {
      "analyzer": {
        "my_custom_analyzer": {
          "tokenizer": "standard",
          "filter": ["lowercase"]
        }
      }
    }
  },
  "mappings": {
    "properties": {
      // ... properties from TestEntity ...
    }
  }
}
```

Refer to `tests/Elasticsearch.FSharp.Tests/Mapping.fs` for more detailed examples, including mapping of recursive types and various attribute usages.

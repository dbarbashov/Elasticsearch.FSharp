module Elasticsearch.FSharp.Mapping.Attributes

open System
open System.Collections.Generic
open System.Runtime.InteropServices
open Elasticsearch.FSharp.Mapping.DSL

/// Use this attribute on types that represent documents in elastic
[<AttributeUsage(AttributeTargets.Class ||| AttributeTargets.Struct)>]
type ElasticType(typeName:string,
                 [<Optional; DefaultParameterValue(false)>] allEnabled:bool) =
    inherit Attribute()
    
    member val TypeName = typeName with get
    member val AllEnabled = allEnabled with get

/// Use this attribute on properties of a type that represents elastic document
[<AttributeUsage(AttributeTargets.Property)>]
type ElasticField([<Optional; DefaultParameterValue(null:string)>] fieldType:string, 
                  [<Optional; DefaultParameterValue(null:string)>] analyzer:string, 
                  [<Optional; DefaultParameterValue(true)>] enabled:bool,
                  [<Optional; DefaultParameterValue(null:string)>] format:string,
                  [<Optional; DefaultParameterValue(0u:uint)>] ignoreAbove:uint,
                  [<Optional; DefaultParameterValue(false)>] useProperties:bool,
                  [<Optional; DefaultParameterValue(10)>] maxDepth:int,
                  [<Optional; DefaultParameterValue(false)>] ignoreMalformed:bool,
                  [<Optional; DefaultParameterValue(null:string)>] name:string) =
    inherit Attribute()
    
    member val FieldType = fieldType with get
    member val Analyzer = analyzer with get
    member val Enabled = enabled with get
    member val IgnoreMalformed = ignoreMalformed with get
    member val Format = format with get
    member val IgnoreAbove = ignoreAbove with get
    member val UseProperties = useProperties with get
    member val MaxDepth = maxDepth with get
    member val Name = name with get

/// Use this field on attributes of a type that should represent additional `fields` of a field
[<AttributeUsage(AttributeTargets.Property, AllowMultiple = true)>]
type ElasticSubField(fieldName: string,
                     [<Optional; DefaultParameterValue(null:string)>] fieldType:string,
                     [<Optional; DefaultParameterValue(null:string)>] analyzer:string,
                     [<Optional; DefaultParameterValue(true)>] enabled:bool,
                     [<Optional; DefaultParameterValue(null:string)>] format:string,
                     [<Optional; DefaultParameterValue(0u:uint)>] ignoreAbove:uint,
                     [<Optional; DefaultParameterValue(false)>] useProperties:bool,
                     [<Optional; DefaultParameterValue(10)>] maxDepth:int,
                     [<Optional; DefaultParameterValue(false)>] ignoreMalformed:bool) =
    inherit ElasticField(fieldType, analyzer, enabled, format, ignoreAbove, useProperties, maxDepth, ignoreMalformed)
    
    member val FieldName = fieldName with get
    
exception UnknownElasticAttributeException of Type
    
exception IsNotElasticTypeException of Type

let private TypeofElasticType = typeof<ElasticType>
    
let rec private getRealType (t: Type) : Type =
    if t.IsArray then
        getRealType (t.GetElementType())
    else if t.Name = typeof<option<_>>.Name then
        getRealType t.GenericTypeArguments.[0]
    else
        t

let fieldToMapping (propAttr: ElasticField) : PropertyMapping =
    { PropertyMapping.Default with 
        Type =
            if String.IsNullOrWhiteSpace propAttr.FieldType then
                None
            else
                Some propAttr.FieldType
        Analyzer =
            if String.IsNullOrWhiteSpace propAttr.Analyzer then
                None
            else
                Some propAttr.Analyzer
        Format =
            if String.IsNullOrWhiteSpace propAttr.Format then
                None
            else
                Some propAttr.Format
        IgnoreAbove =
            if propAttr.IgnoreAbove = 0u then
                None
            else
                Some propAttr.IgnoreAbove
        Enabled = propAttr.Enabled
        IgnoreMalformed = propAttr.IgnoreMalformed
    }
    
let rec private getTypePropertyMappings (t: Type) (depth: int) : Dictionary<string, PropertyMapping> =
    let result = Dictionary<string, PropertyMapping>()
    for prop in t.GetProperties() do
        let propAttributes = prop.GetCustomAttributes(typeof<ElasticField>, true)
        let propType = prop.PropertyType
        
        let elasticFieldAttribute =
            propAttributes
            |> Array.tryFind (fun (p: obj) ->
                match p with
                | :? ElasticSubField -> false
                | :? ElasticField -> true
                | _ -> false)
        
        match elasticFieldAttribute with
        | Some elasticFieldAttribute ->
            let elasticFieldAttribute = elasticFieldAttribute :?> ElasticField
            let mapping = fieldToMapping elasticFieldAttribute
            
            let elasticSubFieldAttributes =
                propAttributes
                |> Array.filter (fun (p: obj) ->
                    match p with
                    | :? ElasticSubField -> true
                    | _ -> false)
                |> Array.map (fun (f: obj) -> f :?> ElasticSubField)
            
            let mapping = 
                if elasticFieldAttribute.UseProperties && depth < elasticFieldAttribute.MaxDepth then
                    let props = getTypePropertyMappings (getRealType propType) (depth+1)
                    { mapping with Properties = Some props }
                else
                    mapping 
            
            let mapping = 
                if elasticSubFieldAttributes.Length > 0 then
                    let subFields = Dictionary<string, PropertyMapping>()
                    for subFieldAttr in elasticSubFieldAttributes do
                        subFields.[subFieldAttr.FieldName] <- fieldToMapping subFieldAttr
                    { mapping with Fields = Some subFields }
                else
                    mapping
            
            let propName =
                match elasticFieldAttribute.Name with
                | null -> prop.Name
                | name -> name
            
            if mapping <> PropertyMapping.Default then 
                result.[propName] <- mapping
        | None ->
            ()
    result
    
let getTypeIndexName (t: Type) = 
    let propAttributes = t.GetCustomAttributes(TypeofElasticType, true)
    let propAttr = Array.tryHead propAttributes
    match propAttr with 
    | Some propAttr ->
        let propAttr = propAttr :?> ElasticType
        propAttr.TypeName
    | None ->
        raise (IsNotElasticTypeException t)
    
let generateElasticMapping (t: Type) : ElasticMapping =
    let attrs = t.GetCustomAttributes(TypeofElasticType, true)
    let attr = Array.tryHead attrs
    match attr with 
    | Some attr ->
        let attr = attr :?> ElasticType
        {
            Settings = None
            Mappings = {
                AllEnabled = Some attr.AllEnabled
                Properties = getTypePropertyMappings t 0
            }
        } 
    | None ->
        raise (IsNotElasticTypeException t)
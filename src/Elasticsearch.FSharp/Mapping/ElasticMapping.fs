namespace Elasticsearch.FSharp
open System

module Mapping = 

    open System.Runtime.InteropServices
    open System.Collections.Generic
    
    
    type ElasticType    (
                            typeName:string,
                            [<Optional; DefaultParameterValue(false)>] allEnabled:bool
                        ) =
        inherit System.Attribute()
         
        member val TypeName = typeName with get
        member val AllEnabled = allEnabled with get
        
    type ElasticField(
                        [<Optional; DefaultParameterValue(null:string)>] fieldType:string, 
                        [<Optional; DefaultParameterValue(null:string)>] analyzer:string, 
                        [<Optional; DefaultParameterValue(true)>] enabled:bool,
                        [<Optional; DefaultParameterValue(null:string)>] format:string,
                        [<Optional; DefaultParameterValue(false)>] useProperties:bool,
                        [<Optional; DefaultParameterValue(10)>] maxDepth:int
                     ) =
        inherit System.Attribute()
        
        member val FieldType        = fieldType     with get
        member val Analyzer         = analyzer      with get
        member val Enabled          = enabled       with get 
        member val Format           = format        with get
        member val UseProperties    = useProperties with get
        member val MaxDepth         = maxDepth      with get
    
    [<AttributeUsage(AttributeTargets.Property, AllowMultiple = true)>]
    type ElasticSubField(
                            fieldName: string,
                            [<Optional; DefaultParameterValue(null:string)>] fieldType:string, 
                            [<Optional; DefaultParameterValue(null:string)>] analyzer:string, 
                            [<Optional; DefaultParameterValue(true)>] enabled:bool,
                            [<Optional; DefaultParameterValue(null:string)>] format:string,
                            [<Optional; DefaultParameterValue(false)>] useProperties:bool,
                            [<Optional; DefaultParameterValue(10)>] maxDepth:int
                        ) =
        inherit ElasticField(fieldType, analyzer, enabled, format, useProperties, maxDepth)
        
        member val FieldName = fieldName with get
        
    module rec ElasticMappingDSL = 
        type ElasticMapping =
            | ElasticMapping of ElasticMappingBody list
            
        and ElasticMappingBody = 
            | Settings of SettingsBody list 
            | Mappings of TypeMapping list
            
        and SettingsBody = 
            | Setting of string*string
            | BoolSetting of string*bool
            
        and TypeMapping =
            string * (TypeMappingBody list)
             
        and TypeMappingBody = 
            | AllEnabled of bool
            | TypeProperties of PropertyMapping list
            
        and PropertyMapping = string * (PropertyMappingField list)
        
        and PropertyMappingField = 
            | Type of string 
            | Analyzer of string 
            | Enabled of bool
            | Format of string
            | Properties of PropertyMapping list
            | Fields of PropertyMapping list
            
        let internal SettingsBodyListToJSON settingsBody =
            "{" + 
                ([
                    for sb in settingsBody -> 
                        match sb with 
                        | Setting (k, v) ->
                            "\""+k+"\":" + "\""+v+"\"" 
                        | BoolSetting (k, v) -> 
                            "\""+k+"\":" + if v then "true" else "false"
                ] |> String.concat ",")
            + "}"
            
        let internal PropertyMappingFieldListToJSON propertyMappingFields = 
            "{" + 
                ([
                    for pmf in propertyMappingFields do 
                        match pmf with 
                        | Type v -> 
                            yield "\"type\":\"" + v + "\"" 
                        | Analyzer v -> 
                            yield "\"analyzer\":\"" + v + "\"" 
                        | Enabled v -> 
                            yield "\"enabled\":" + (if v then "true" else "false")
                        | Format v -> 
                            yield "\"format\":\"" + v + "\""
                        | Properties v -> 
                            yield "\"properties\":{" + (PropertyMappingListToJSON v) + "}"
                        | Fields v->
                            match v with
                            | [] ->
                                ()
                            | _ -> 
                                yield "\"fields\":{" + (PropertyMappingListToJSON v) + "}"
                ] |> String.concat ",")
            + "}"
            
        let internal PropertyMappingListToJSON propertyMappings = 
            ([
                for (fieldName, pmf) in propertyMappings ->
                    "\"" + fieldName + "\":" + (PropertyMappingFieldListToJSON pmf)
            ] |> String.concat ",")         
            
        let internal TypeMappingBodyListToJSON typeMappingsBody = 
            "{" + 
                ([
                    for tm in typeMappingsBody ->
                        match tm with 
                        | AllEnabled v -> 
                            "\"_all\":{\"enabled\":" + (if v then "true" else "false") + "}"
                        | TypeProperties propertyMappings -> 
                           "\"properties\":{" + PropertyMappingListToJSON propertyMappings + "}" 
                ] |> String.concat ",")
            + "}"
            
        let internal MappingsBodyListToJSON mappingsBody = 
            "{" + 
                ([
                    for (typeName, typeMappingsBody) in mappingsBody -> 
                        "\"" + typeName + "\":" + (TypeMappingBodyListToJSON typeMappingsBody)
                ] |> String.concat ",")
            + "}"
            
        let internal ElasticMappingBodyListToJSON mappingBodies =
            [
                for mb in mappingBodies ->
                    match mb with 
                    | Settings settingsBody -> 
                        "\"settings\":" + (SettingsBodyListToJSON settingsBody)
                    | Mappings mappingsBody ->
                        "\"mappings\":" + (MappingsBodyListToJSON mappingsBody)
            ] |> String.concat ","

        let private PrevPropNameDefault propMapping = [propMapping]
        
        let rec private GetSingularTypePropertiesMappings
                (prevPropName:PropertyMapping -> PropertyMapping list)
                (propMappings: PropertyMapping list)
                (outContainer: ref<List<PropertyMapping list>>) =
            for (propName, propDef) in propMappings do
                let mutable hasProperties = false
                for propMappingField in propDef do
                    match propMappingField with
                    | Properties props ->
                        hasProperties <- true
                        let fn propMapping = prevPropName (propName, [Properties [propMapping]])
                        GetSingularTypePropertiesMappings fn props outContainer
                    | _ -> ()
                if not <| hasProperties then
                    outContainer.Value.Add(prevPropName (propName, propDef))

        let ElasticMappingToPutMappingJSON mapping =
            [
                match mapping with
                | ElasticMapping mappingBody ->
                    for mb in mappingBody do
                        match mb with
                        | Mappings mappingsBody ->
                            for (typeName, typeMappingsBody) in mappingsBody do
                                for tm in typeMappingsBody do
                                    match tm with
                                    | TypeProperties propertyMappings ->
                                       let propList = ref (new List<PropertyMapping list>())
                                       GetSingularTypePropertiesMappings PrevPropNameDefault propertyMappings propList
                                       for propertyMappings in propList.Value do
                                           yield
                                               "{\"properties\":{" + PropertyMappingListToJSON propertyMappings + "}}"
                                    | _ -> ()
                                    
                        | _ -> ()
            ]
            
        let ElasticMappingToJSON mapping =
            "{" + 
                match mapping with 
                | ElasticMapping mappingBody ->
                    ElasticMappingBodyListToJSON mappingBody
            + "}"
    
    open ElasticMappingDSL
    
    let private typeofElasticType = typeof<ElasticType>
        
    let rec private GetRealType (t:System.Type) : System.Type =
        if t.IsArray then
            GetRealType (t.GetElementType())
        else if t.Name = typeof<option<_>>.Name then
            GetRealType t.GenericTypeArguments.[0]
        else
            t
    
    let rec FieldToMapping (propAttr: ElasticField) : PropertyMappingField list =
        (seq {
            if propAttr.Enabled then 
                yield PropertyMappingField.Type propAttr.FieldType
            
            if not <| String.IsNullOrWhiteSpace propAttr.Analyzer then   
                yield Analyzer propAttr.Analyzer
                
            if not propAttr.Enabled then 
                yield Enabled false
                
            if not <| String.IsNullOrWhiteSpace propAttr.Format then
                yield Format propAttr.Format
                
        } |> Seq.toList)
    
    exception UnknownElasticAttributeException of System.Type
        
    let rec private GetTypePropertyMappings (t:System.Type) (depth:int) : PropertyMapping list = 
        [
            for prop in t.GetProperties() do
                let propAttributes = prop.GetCustomAttributes(typeof<ElasticField>, true)
                let propType = prop.PropertyType
                
                let props =
                    [
                        for propAttr in propAttributes do
                        match propAttr with
                        | :? ElasticSubField ->
                            ()
                        | :? ElasticField as propAttr ->
                            if propAttr.UseProperties then
                                if depth < propAttr.MaxDepth then  
                                    let subTypeProps = GetTypePropertyMappings (GetRealType propType) (depth+1)
                                    yield 
                                        Properties subTypeProps 
                            else 
                                yield! (propAttr |> FieldToMapping)
                        | _ ->
                            raise(UnknownElasticAttributeException(propAttr.GetType()))
                    ]

                if props.Length > 0 then
                    let fields =
                        propAttributes
                        |> Array.choose (fun a -> match a with | :? ElasticSubField as f -> Some f | _ -> None)
                        |> Array.map
                               (fun propAttr ->
                                    propAttr.FieldName, propAttr |> FieldToMapping
                               )
                        |> Array.toList
                    
                    yield prop.Name, [
                        if fields.Length > 0 then
                            yield fields |> Fields
                        yield! props
                    ]
        ]
        
    exception IsNotElasticTypeException of System.Type
        
    let GetTypeIndexName (t:System.Type) = 
        let propAttributes = t.GetCustomAttributes(typeofElasticType, true)
        let propAttr = Array.tryHead propAttributes
        match propAttr with 
        | Some propAttr ->
            let propAttr = propAttr :?> ElasticType
            propAttr.TypeName
        | None ->
            raise (IsNotElasticTypeException t)
        
    let GenerateElasticMappings(t:System.Type) : TypeMapping =
        let attrs = t.GetCustomAttributes(typeofElasticType, true)
        let attr = Array.tryHead attrs
        match attr with 
        | Some attr -> 
            let attr = attr :?> ElasticType
            let propertyMappings = GetTypePropertyMappings t 0
            
            attr.TypeName, [
                TypeMappingBody.TypeProperties propertyMappings
            ]
        | None -> 
            raise (System.Exception "ElasticType attribute not found!")
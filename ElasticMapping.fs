namespace Elasticsearch.FSharp

module Mapping = 

    open System
    open System.Runtime.InteropServices
    open System.Reflection
    
    
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
                        [<Optional; DefaultParameterValue(false)>] useProperties:bool
                     ) =
        inherit System.Attribute()
        
        member val FieldType        = fieldType     with get
        member val Analyzer         = analyzer      with get
        member val Enabled          = enabled       with get 
        member val Format           = format        with get
        member val UseProperties    = useProperties with get
        
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
                    for pmf in propertyMappingFields -> 
                        match pmf with 
                        | Type v -> 
                            "\"type\":\"" + v + "\"" 
                        | Analyzer v -> 
                            "\"analyzer\":\"" + v + "\"" 
                        | Enabled v -> 
                            "\"enabled\":" + (if v then "true" else "false")
                        | Format v -> 
                            "\"format\":\"" + v + "\""
                        | Properties v -> 
                            "\"properties\":{" + (PropertyMappingListToJSON v) + "}"
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
            
        let ElasticMappingToJSON mapping =
            "{" + 
                match mapping with 
                | ElasticMapping mappingBody ->
                    ElasticMappingBodyListToJSON mappingBody
            + "}"
    
    open ElasticMappingDSL
    
    let private typeofElasticField = typeof<ElasticField>
    let private typeofElasticType = typeof<ElasticType>
        
    let rec private GetRealType (t:System.Type) : System.Type =
        if t.IsArray then
            GetRealType (t.GetElementType())
        else if t.Name = typeof<option<_>>.Name then
            GetRealType t.GenericTypeArguments.[0]
        else
            t
        
    let rec private GetTypePropertyMappings (t:System.Type) : PropertyMapping list = 
        [
            for prop in t.GetProperties() do
                let propAttributes = prop.GetCustomAttributes(typeofElasticField, true)
                let propAttr = Array.tryHead propAttributes
                
                match propAttr with 
                | Some propAttr ->
                    let propAttr = propAttr :?> ElasticField
                    let propType = prop.PropertyType
                    
                    if propAttr.UseProperties then
                        let subTypeProps = GetTypePropertyMappings (GetRealType propType)
                        yield 
                            prop.Name, [
                                Properties subTypeProps
                            ] 
                    else 
                        yield 
                            prop.Name, (seq {
                                if propAttr.Enabled then 
                                    yield PropertyMappingField.Type propAttr.FieldType
                                
                                if not <| String.IsNullOrWhiteSpace propAttr.Analyzer then   
                                    yield Analyzer propAttr.Analyzer
                                    
                                if not propAttr.Enabled then 
                                    yield Enabled false
                                    
                                if not <| String.IsNullOrWhiteSpace propAttr.Format then
                                    yield Format propAttr.Format
                                    
                            } |> Seq.toList)
                | None -> 
                    ()
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
            let propertyMappings = GetTypePropertyMappings t
            
            attr.TypeName, [
                AllEnabled attr.AllEnabled
                TypeMappingBody.TypeProperties propertyMappings
            ]
        | None -> 
            raise (System.Exception "ElasticType attribute not found!")
module internal Elasticsearch.FSharp.Utility

[<RequireQualifiedAccess>]
module Json = 
    let inline quoteString (s: string) = $"\"{s}\""
    let inline boolToString (b: bool) = if b then "true" else "false"
    let inline makeKeyValue (key: string) (value: string) = $"{quoteString key}:{value}"
    let inline makeObject innerParts =
        let middle = innerParts |> String.concat ","
        $"{{{middle}}}"
    let inline makeArray innerParts = 
        let middle = innerParts |> String.concat ","
        $"[{middle}]"
    let inline makeQuotedArray strings =
        strings
        |> (Seq.map quoteString >> makeArray)
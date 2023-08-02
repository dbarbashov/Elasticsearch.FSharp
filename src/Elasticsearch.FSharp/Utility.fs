module internal Elasticsearch.FSharp.Utility

[<RequireQualifiedAccess>]
module Json = 
    open System
    let escapeChar c =
        match c with
        | '"' -> "\\\""
        | '\\' -> "\\\\"
        | '/' -> "\\/"
        | '\t' -> "\\t"
        | '\b' -> "\\b"
        | '\r' -> "\\r"
        | '\n' -> "\\n"
        | '\f' -> "\\f"
        | _ when int c >= 32 -> c.ToString()
        | _ -> $"\\u{int c:x4}"
    let escapeString (s: string) =
        if String.IsNullOrEmpty s |> not then
            s.ToCharArray()
            |> Array.map escapeChar
            |> String.concat ""
        else
            s
    let inline quoteString (s: string) = $"\"{escapeString s}\""
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
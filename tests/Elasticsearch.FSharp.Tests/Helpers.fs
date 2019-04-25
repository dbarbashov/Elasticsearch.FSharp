module internal Elasticsearch.FSharp.Tests.Helpers

open System.Text.RegularExpressions

let removeWhitespace s =
    Regex.Replace(s, "\s+", System.String.Empty)
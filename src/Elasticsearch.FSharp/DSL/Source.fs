namespace Elasticsearch.FSharp.DSL

type Includes = string list
type Excludes = string list

type SourceBody =
    | Nothing
    | Only of string
    | List of string list
    | Pattern of Includes: Includes * Excludes: Excludes
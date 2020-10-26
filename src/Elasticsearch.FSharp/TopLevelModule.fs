module Elasticsearch.FSharp.Module

open Elasticsearch.FSharp.Builders

let elastic<'T when 'T: not struct>() = ElasticBuilder<'T>()

let match_all() = Query.MatchAllBuilder().Zero()

let bool<'T when 'T: not struct> = Query.BoolBuilder<'T>()

let agg name = Aggs.AggBuilder name

let terms = Aggs.TermsBuilder()

let sortBy = Sort.SortByBuilder()
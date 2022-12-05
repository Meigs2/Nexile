using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nexile.PathOfExile.QueryBuilder;

public record TradeQuery
{
    [JsonProperty("query")] public Query Query { get; init; } = new();
    [JsonProperty("sort")] public Sort Sort { get; init; } = new();
}

public record Query
{
    [JsonProperty("status", Order = 0)] public Status Status { get; init; } = new();

    [JsonProperty("name", Order = 1, NullValueHandling = NullValueHandling.Ignore)]
    public string? ItemName { get; init; }

    [JsonProperty("type", Order = 2, NullValueHandling = NullValueHandling.Ignore)]
    public string? ItemType { get; init; }

    [JsonProperty("stats", Order = 3)]
    public List<Stat> Stats = new List<Stat> { new() { Type = "and" } };

    [JsonProperty("filters", Order = 4, NullValueHandling = NullValueHandling.Ignore)]
    public List<Filter> Filters { get; init; }
}

public record Sort
{
    [JsonProperty("price")] public string Price { get; init; } = "asc";
}

public record Filter
{
    [JsonProperty("id")] public string Id { get; init; }
    [JsonProperty("disabled")] public bool Disabled { get; init; }
    [JsonProperty("value")] public Value Value { get; init; }
}

public record Stat
{
    [JsonProperty("type")] public string Type { get; init; }
    [JsonProperty("filters")] public List<Filter> Filters { get; init; }

    [JsonProperty("disabled", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Disabled { get; init; }

    [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
    public Value? Value { get; init; }
}

public record Status
{
    [JsonProperty("option")] public string Option { get; init; } = "online";
}

public record Value
{
    [JsonProperty("min")] public double Min { get; init; }
    [JsonProperty("max")] public double Max { get; init; }
    [JsonProperty("weight")] public double? Weight { get; init; }
}
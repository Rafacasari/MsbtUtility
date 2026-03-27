using MsbtUtility.Converters;
using Newtonsoft.Json;
using System.ComponentModel;

namespace MsbtUtility.TagMap;

public class TagInfo
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue("")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("group")]
    public int Group { get; set; } = -1;

    [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue(-1)]
    public int Type { get; set; } = -1;

    [JsonProperty("types"), IgnoreEmptyCollection]
    public int[] Types { get; set; } = [];

    [JsonProperty("typeMap"), IgnoreEmptyCollection]
    public Dictionary<string, ValueInfo> TypeMap { get; set; } = [];

    [JsonProperty("discard", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool Discard { get; set; }

    [JsonProperty("arguments"), IgnoreEmptyCollection]
    public List<ArgumentInfo> Arguments { get; set; } = [];
}

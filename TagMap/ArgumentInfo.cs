using MsbtUtility.Converters;
using Newtonsoft.Json;
using System.ComponentModel;

namespace MsbtUtility.TagMap;

public class ArgumentInfo
{
    [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue("")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue("")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("dataType")]
    public string DataType { get; set; } = string.Empty;

    [JsonProperty("valueMap"), IgnoreEmptyCollection]
    public Dictionary<string, ValueInfo> ValueMap { get; set; } = [];

    [JsonProperty("arrayLength", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int ArrayLength { get; set; }
}
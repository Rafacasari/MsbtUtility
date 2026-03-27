using MsbtUtility.Converters;
using Newtonsoft.Json;

namespace MsbtUtility.TagMap;

[JsonConverter(typeof(ValueInfoConverter))]
public class ValueInfo
{
    [JsonProperty("name")]
    public required string Name { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;
}

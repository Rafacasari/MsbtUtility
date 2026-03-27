using MsbtUtility.TagMap;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MsbtUtility.Converters;

internal sealed class ValueInfoConverter : JsonConverter<ValueInfo>
{
    public override bool CanRead => true;

    public override bool CanWrite => true;

    public override ValueInfo? ReadJson(JsonReader reader, Type objectType, ValueInfo? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        switch (reader.TokenType)
        {
            case JsonToken.Null:
                return existingValue;
            case JsonToken.String:
                return new ValueInfo { Name = (string)reader.Value! };
            case JsonToken.StartObject:
                var obj = JObject.Load(reader);
                ValueInfo info;
                if (obj.TryGetValue("name", StringComparison.OrdinalIgnoreCase, out var name))
                {
                    info = new ValueInfo { Name = name.Value<string>()! };
                }
                else if (obj.TryGetValue("value", StringComparison.OrdinalIgnoreCase, out name))
                {
                    info = new ValueInfo { Name = name.Value<string>()! };
                }
                else throw new JsonReaderException("Missing value name.");
                if (obj.TryGetValue("description", StringComparison.OrdinalIgnoreCase, out var description))
                {
                    info.Description = description.Value<string>() ?? string.Empty;
                }
                return info;
            default:
                throw new JsonReaderException("Invalid type for value.");
        }
    }

    public override void WriteJson(JsonWriter writer, ValueInfo? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }

        if (string.IsNullOrEmpty(value.Description))
        {
            writer.WriteValue(value.Name);
        }
        else
        {
            writer.WriteStartObject();
            writer.WritePropertyName("name");
            writer.WriteValue(value.Name);
            writer.WritePropertyName("description");
            writer.WriteValue(value.Description);
            writer.WriteEndObject();
        }
    }
}

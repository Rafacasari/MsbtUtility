using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Numerics;

namespace MsbtUtility.Converters;

internal sealed class NumberConverter<T> : JsonConverter<T?> where T : struct, INumber<T>
{
    public override bool CanRead => true;

    public override bool CanWrite => true;

    public override T? ReadJson(JsonReader reader, Type objectType, T? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType is JsonToken.Null) return existingValue;
        if (reader.TokenType is JsonToken.Integer) return JToken.ReadFrom(reader).ToObject<T>();
        if (reader.TokenType is JsonToken.String)
        {
            var value = (string?)reader.Value;
            if (value is null) return existingValue;

            if (value.StartsWith("0x") || value.StartsWith("0X")) return T.Parse(value.AsSpan(2), NumberStyles.AllowHexSpecifier, null);
            return T.Parse(value.AsSpan(), NumberStyles.Integer, null);
        }

        throw new JsonSerializationException($"Unsupported token type {reader.TokenType}.");
    }

    public override void WriteJson(JsonWriter writer, T? value, JsonSerializer serializer)
    {
        if (value.HasValue) writer.WriteValue(value);
        else writer.WriteNull();
    }
}

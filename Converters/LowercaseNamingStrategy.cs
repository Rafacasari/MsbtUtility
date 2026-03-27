using Newtonsoft.Json.Serialization;

namespace MsbtUtility.Converters;

internal class LowercaseNamingStrategy : NamingStrategy
{
    public static readonly LowercaseNamingStrategy Instance = new();

    protected override string ResolvePropertyName(string name) => name.ToLowerInvariant();
}

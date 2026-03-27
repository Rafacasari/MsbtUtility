using Newtonsoft.Json.Converters;

namespace MsbtUtility.Converters;


internal class LowercaseStringEnumConverter() : StringEnumConverter(LowercaseNamingStrategy.Instance);
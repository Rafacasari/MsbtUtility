using MsbtUtility.Converters;
using Newtonsoft.Json;
using YamlConverter;

namespace MsbtUtility;

public static class ConfigManager
{
    public static GameConfig? CurrentConfig { get; private set; }

    static ConfigManager()
    {
        CurrentConfig = LoadConfig();
    }

    internal static readonly JsonSerializerSettings JsonSettings = new()
    {
        ContractResolver = ContractResolver.Instance,
        Converters =
        [
           new NumberConverter<byte>(),
            new NumberConverter<sbyte>(),
            new NumberConverter<short>(),
            new NumberConverter<ushort>(),
            new NumberConverter<int>(),
            new NumberConverter<uint>(),
            new NumberConverter<long>(),
            new NumberConverter<ulong>()
        ]
    };


    public static GameConfig LoadConfig()
    {
        if (!File.Exists("TomoLife_LtD.gcf"))
            throw new Exception("Config file not found!");

        using var stream = File.OpenRead("TomoLife_LtD.gcf");
        using var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();
        var config = YamlConvert.DeserializeObject<GameConfig>(content, JsonSettings) ?? new GameConfig();
        config.Validate();

        return config;
    }
}
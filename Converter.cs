using System.Text.Json;
using System.Text.Json.Serialization;
using AeonSake.NintendoTools.FileFormats;
using AeonSake.NintendoTools.FileFormats.Msbt;

namespace MsbtUtility;

public static class Converter
{
    #region public methods
    public static (string metaJson, string textJson) Serialize(MsbtFile file)
    {
        var metaData = new MetaData
        {
            BigEndian = file.BigEndian,
            BigEndianLabels = file.BigEndianLabels,
            Version = file.Version,
            EncodingType = file.EncodingType,
            HasNli1 = file.HasNli1,
            HasLbl1 = file.HasLbl1,
            LabelGroups = file.LabelGroups,
            HasAtr1 = file.HasAtr1,
            AdditionalAttributeData = file.AdditionalAttributeData,
            HasAto1 = file.HasAto1,
            Ato1Data = file.Ato1Data,
            HasTsy1 = file.HasTsy1,
            HasTxtW = file.HasTxtW
        };
        var textData = new Dictionary<string, string>();

        for (var i = 0; i < file.Messages.Count; ++i)
        {
            var message = file.Messages[i];
            var key = file.HasLbl1 ? message.Label : file.HasNli1 ? message.Id.ToString() : i.ToString();

            metaData.Messages.Add(key, new MessageMetaData
            {
                Attribute = message.Attribute,
                AttributeText = message.AttributeText,
                StyleIndex = message.StyleIndex
            });
            textData.Add(key, message.Text);
        }

        var metaJson = JsonSerializer.Serialize(metaData);
        var textJson = JsonSerializer.Serialize(textData);
        return (metaJson, textJson);
    }

    public static MsbtFile Deserialize(string metaJson, string textJson)
    {
        var metaData = JsonSerializer.Deserialize<MetaData>(metaJson) ?? throw new Exception("Failed to Deserialize .meta.json");
        var textData = JsonSerializer.Deserialize<Dictionary<string, string>>(textJson) ?? throw new Exception("Failed to .text.json!");

        var file = new MsbtFile
        {
            BigEndian = metaData.BigEndian,
            BigEndianLabels = metaData.BigEndianLabels,
            Version = metaData.Version,
            EncodingType = metaData.EncodingType,
            HasNli1 = metaData.HasNli1,
            HasLbl1 = metaData.HasLbl1,
            LabelGroups = metaData.LabelGroups,
            HasAtr1 = metaData.HasAtr1,
            AdditionalAttributeData = metaData.AdditionalAttributeData,
            HasAto1 = metaData.HasAto1,
            Ato1Data = metaData.Ato1Data,
            HasTsy1 = metaData.HasTsy1,
            HasTxtW = metaData.HasTxtW
        };

        foreach (var (key, text) in textData)
        {
            var messageMeta = metaData.Messages[key];

            var message = new MsbtMessage
            {
                Attribute = messageMeta.Attribute,
                AttributeText = messageMeta.AttributeText,
                StyleIndex = messageMeta.StyleIndex,
                Text = text
            };

            if (metaData.HasLbl1)
            {
                message.Label = key;
            }
            else if (metaData.HasNli1)
            {
                message.Id = uint.Parse(key);
            }

            file.Messages.Add(message);
        }

        return file;
    }

    public static MsbtFile Deserialize(MsbtFile baseFile, string textJson)
    {
        var textData = JsonSerializer.Deserialize<Dictionary<string, string>>(textJson) ?? throw new Exception("Failed to Deserialize json!");
        var file = new MsbtFile
        {
            BigEndian = baseFile.BigEndian,
            BigEndianLabels = baseFile.BigEndianLabels,
            Version = baseFile.Version,
            EncodingType = baseFile.EncodingType,
            HasNli1 = baseFile.HasNli1,
            HasLbl1 = baseFile.HasLbl1,
            LabelGroups = baseFile.LabelGroups,
            HasAtr1 = baseFile.HasAtr1,
            AdditionalAttributeData = (byte[])baseFile.AdditionalAttributeData.Clone(),
            HasAto1 = baseFile.HasAto1,
            Ato1Data = baseFile.Ato1Data,
            HasTsy1 = baseFile.HasTsy1,
            HasTxtW = baseFile.HasTxtW
        };

        for (var i = 0; i < baseFile.Messages.Count; ++i)
        {
            var baseMessage = baseFile.Messages[i];
            var key = file.HasLbl1 ? baseMessage.Label : file.HasNli1 ? baseMessage.Id.ToString() : i.ToString();

            var message = new MsbtMessage
            {
                Attribute = baseMessage.Attribute,
                AttributeText = baseMessage.AttributeText,
                StyleIndex = baseMessage.StyleIndex,
                Text = textData[key]
            };
            file.Messages.Add(message);
        }

        return file;
    }
    #endregion

    #region helper classes
    private class MetaData
    {
        public bool BigEndian { get; init; }

        public bool BigEndianLabels { get; init; }

        public int Version { get; init; }

        [JsonConverter(typeof(JsonStringEnumConverter<EncodingType>))]
        public EncodingType EncodingType { get; init; }

        public bool HasNli1 { get; init; }

        public bool HasLbl1 { get; init; }

        public int LabelGroups { get; init; }

        public bool HasAtr1 { get; init; }

        public byte[] AdditionalAttributeData { get; init; } = [];

        public bool HasAto1 { get; init; }

        public byte[] Ato1Data { get; init; } = [];

        public bool HasTsy1 { get; init; }

        public bool HasTxtW { get; init; }

        public Dictionary<string, MessageMetaData> Messages { get; init; } = [];
    }

    private class MessageMetaData
    {
        public byte[] Attribute { get; init; } = [];

        public string? AttributeText { get; init; }

        public uint StyleIndex { get; init; }
    }
    #endregion
}
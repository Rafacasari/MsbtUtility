using AeonSake.BinaryTools.Converters;
using AeonSake.NintendoTools.FileFormats;
using AeonSake.NintendoTools.FileFormats.Msbt;
using MsbtUtility.Converters;
using MsbtUtility.TagMap;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace MsbtUtility;

public partial class MsbtConfig
{
    #region public properties
    [JsonProperty("fixedLabelGroups", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool FixedLabelGroups { get; set; }

    [JsonProperty("newline", DefaultValueHandling = DefaultValueHandling.Ignore), JsonConverter(typeof(LowercaseStringEnumConverter)), DefaultValue(NewlineType.LF)]
    public NewlineType Newline { get; set; } = NewlineType.LF;

    [JsonProperty("tags"), IgnoreEmptyCollection]
    public List<TagInfo> Tags { get; set; } = [];

    [JsonIgnore]
    public MsbtTagInfo[] TagInfos { get; set; } = [];

    [JsonIgnore]
    public IMsbtTagFormatter TagFormatter { get; set; } = new MsbtDefaultTagFormatter();
    #endregion

    #region public methods
    public void Validate()
    {
        var tagInfos = new List<MsbtTagInfo>();
        foreach (var tag in Tags)
        {
            if (!NameRegex().IsMatch(tag.Name)) throw new InvalidDataException($"The tag name \"{tag.Name}\" contains invalid characters.");
            if (tag.Group is < 0 or > ushort.MaxValue) throw new IndexOutOfRangeException($"Tag group ID of {tag.Name} is out of range (must be between 0 and 65535).");
            if (tag.Type > ushort.MaxValue) throw new IndexOutOfRangeException($"Tag type ID of {tag.Name} is out of range (must be between 0 and 65535).");

            var typeList = new ushort[tag.Types.Length];
            for (var i = 0; i < typeList.Length; ++i)
            {
                var value = tag.Types[i];
                if (value is < 0 or > ushort.MaxValue) throw new IndexOutOfRangeException($"Tag type ID of {tag.Name} is out of range (must be between 0 and 65535).");
                typeList[i] = (ushort)value;
            }

            var typeMap = new List<MsbtValueInfo>(tag.TypeMap.Count);
            foreach (var (key, info) in tag.TypeMap)
            {
                if (!NameRegex().IsMatch(info.Name)) throw new InvalidDataException($"The type map item \"{info.Name}\" contains invalid characters.");

                typeMap.Add(new MsbtValueInfo
                {
                    Value = key,
                    Name = info.Name,
                    Description = info.Description
                });
            }

            var arguments = new List<MsbtArgumentInfo>(tag.Arguments.Count);
            foreach (var argument in tag.Arguments)
            {
                if (!string.IsNullOrEmpty(argument.Name) && !NameRegex().IsMatch(argument.Name)) throw new InvalidDataException($"The argument name \"{argument.Name}\" contains invalid characters.");
                if (!MsbtDataTypes.TryGetType(argument.DataType, out var dataType) && TryGetPaddingType(argument.DataType, out dataType)) throw new InvalidDataException($"\"{argument.DataType}\" is not a valid MSBT data type.");

                var valueMap = new List<MsbtValueInfo>(argument.ValueMap.Count);
                foreach (var (key, info) in argument.ValueMap)
                {
                    if (info.Name.Contains('"')) throw new InvalidDataException($"The value map item \"{info.Name}\" contains invalid characters.");

                    valueMap.Add(new MsbtValueInfo
                    {
                        Value = key,
                        Name = info.Name,
                        Description = info.Description
                    });
                }

                arguments.Add(new MsbtArgumentInfo
                {
                    DataType = dataType,
                    ValueMap = MsbtValueMap.Create(valueMap, dataType),
                    ArrayLength = argument.ArrayLength < 0 ? 0 : argument.ArrayLength,
                    Name = string.IsNullOrEmpty(argument.Name) ? $"arg{arguments.Count + 1}" : argument.Name,
                    Description = argument.Description
                });
            }

            tagInfos.Add(new MsbtTagInfo
            {
                GroupId = (ushort)tag.Group,
                TypeId = tag.Type > -1 ? (ushort)tag.Type : null,
                TypeIds = typeList,
                TypeMap = MsbtValueMap.Create(typeMap, MsbtDataTypes.UInt16),
                HasDiscard = tag.Discard,
                Name = tag.Name,
                Description = tag.Description,
                Arguments = arguments
            });
        }

        TagInfos = [.. tagInfos];
        TagFormatter = MsbtNamedTagFormatter.Create(TagInfos);
    }

    private static bool TryGetPaddingType(string value, out MsbtDataType dataType)
    {
        dataType = null!;
        if (!HexConverter.TryFromHexString(value, out var padding)) return false;
        dataType = new MsbtPaddingDataType(padding);
        return true;
    }

    [GeneratedRegex(@"^[A-Za-z0-9_\-.]+$")]
    private static partial Regex NameRegex();
    #endregion
}
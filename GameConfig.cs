using Newtonsoft.Json;
using System.ComponentModel;

namespace MsbtUtility;

public class GameConfig
{
    #region general data
    [JsonProperty("game", DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue("")]
    public string Game { get; set; } = string.Empty;
    #endregion

    #region file-specific configs
    //[JsonProperty("zstd"), IgnoreEmptyCollection]
    //public SingleOrArray<ZstdConfig> Zstd { get; set; } = [];


    //[JsonProperty("sarc"), IgnoreEmptyCollection]
    //public SingleOrArray<SarcConfig> Sarc { get; set; } = [];


    [JsonProperty("msbt")]
    public MsbtConfig Msbt { get; set; } = new();
    #endregion

    #region public methods
    public void Validate()
    {
        //Zstd.Validate();
        //Sarc.Validate();
        Msbt.Validate();
    }
    #endregion
}

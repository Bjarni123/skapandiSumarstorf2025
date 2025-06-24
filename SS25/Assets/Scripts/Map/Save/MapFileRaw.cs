using System.Collections.Generic;
using Newtonsoft.Json;

public class MapFileRaw
{
    [JsonProperty("tiles")]
    public List<string> Tiles;
    [JsonProperty("objects")]
    public Dictionary<string, MapFileObject> Objects;
    [JsonProperty("monsters")]
    public Dictionary<string, MapFileMonster> Monsters;
    [JsonProperty("players")]
    public Dictionary<string, MapFilePlayer> Players;
}
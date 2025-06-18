using System.Collections.ObjectModel;
using Newtonsoft.Json;

public class MapFile 
{
  [JsonProperty("tiles")]
  public ReadOnlyCollection<ReadOnlyCollection<MapFileTile>> Tiles;
  [JsonProperty("objects")]
  public ReadOnlyDictionary<string, MapFileObject> Objects;
  [JsonProperty("monsters")]
  public ReadOnlyDictionary<string, MapFileMonster> Monsters;
  [JsonProperty("players")]
  public ReadOnlyDictionary<string, MapFilePlayer> Players;
}
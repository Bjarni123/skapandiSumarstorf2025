using Newtonsoft.Json;
using UnityEngine;

public class MapFileMonster
{
    [JsonProperty("type")]
    public MapFileMonsterType Type;
    [JsonProperty("position")]
    public Vector2 Position;
}
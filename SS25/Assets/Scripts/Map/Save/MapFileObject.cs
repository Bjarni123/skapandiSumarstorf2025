using Newtonsoft.Json;
using UnityEngine;

public class MapFileObject
{
    [JsonProperty("type")]
    public MapFileObjectType Type;
    [JsonProperty("position")]
    public Vector2 Position;
}
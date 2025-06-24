using Newtonsoft.Json;
using UnityEngine;

public class MapFilePlayer
{
    [JsonProperty("position")]
    public Vector2 Position;
}
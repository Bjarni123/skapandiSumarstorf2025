using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MapLoader 
{
    public static MapFile File { get; private set; }
    public static MapTilesLoader TilesLoader { get; set; }
    public static MapObjectsLoader ObjectsLoader { get; set; }
    public static MapMonstersLoader MonstersLoader { get; set; }
    public static MapPlayersLoader PlayersLoader { get; set; }

    static MapLoader() {
        var jsonFile = Resources.Load<TextAsset>($"Maps/Example");
        File = JsonConvert.DeserializeObject<MapFile>(jsonFile.text);
    }
    
    public static void Load(string json) {
        var jsonFile = Resources.Load<TextAsset>($"Maps/{json}");
        File = JsonConvert.DeserializeObject<MapFile>(jsonFile.text);
        SceneManager.LoadScene("Scene/Map");
    }
}
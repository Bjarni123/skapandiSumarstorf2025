
using System.Collections.Generic;

public class MapFile
{
    public MapFileTile[,] Tiles;
    public Dictionary<string, MapFileObject> Objects;
    public Dictionary<string, MapFileMonster> Monsters;
    public Dictionary<string, MapFilePlayer> Players;

    public MapFile()
    {
        Tiles = new MapFileTile[0, 0];
        Objects = new Dictionary<string, MapFileObject>();
        Monsters = new Dictionary<string, MapFileMonster>();
        Players = new Dictionary<string, MapFilePlayer>();
    }

    public MapFile(MapFileRaw raw)
    {
        var height = raw.Tiles.Count;
        var width = raw.Tiles[0].Split(' ', System.StringSplitOptions.RemoveEmptyEntries).Length;
        Tiles = new MapFileTile[height, width];
        for (var y = 0; y < height; y++)
        {
            var splits = raw.Tiles[y].Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            for (var x = 0; x < width; x++)
            {
                switch (splits[x])
                {
                    case "G":
                        Tiles[y, x] = MapFileTile.Grass;
                        break;
                    case "S":
                        Tiles[y, x] = MapFileTile.Sand;
                        break;
                    case "W":
                        Tiles[y, x] = MapFileTile.Water;
                        break;
                    default:
                        UnityEngine.Debug.LogWarning($"Unknown tile type: {splits[x]}");
                        Tiles[y, x] = MapFileTile.None;
                        break;
                }
            }
        }
        Objects = raw.Objects;
        Monsters = raw.Monsters;
        Players = raw.Players;
    }
}
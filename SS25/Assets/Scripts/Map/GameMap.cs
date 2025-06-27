using System;
using System.IO;
using UnityEngine;

public static class GameMap
{
    private static Texture2D texture;
    public static int Width { get => texture.width; }
    public static int Height { get => texture.height; }

    public delegate void MapChunkOnChange(int x, int y);
    private static MapChunkOnChange onChange;

    public static GameMapTile GetTile(int x, int y)
    {
        Color pixelColor = texture.GetPixel(x, y);
        int r = (int)(pixelColor.r * 255);
        int g = (int)(pixelColor.g * 255);
        int b = (int)(pixelColor.b * 255);
        return (r, g, b) switch
        {
            (28, 135, 194) => GameMapTile.Water,
            (30, 123, 184) => GameMapTile.Water,
            (218, 190, 148) => GameMapTile.Sand,
            (224, 191, 142) => GameMapTile.Sand,
            (230, 192, 136) => GameMapTile.Sand,
            (252, 239, 192) => GameMapTile.Sand,
            (255, 241, 188) => GameMapTile.Sand,
            (61, 117, 93) => GameMapTile.Grass,
            (63, 151, 101) => GameMapTile.Grass,
            (68, 110, 92) => GameMapTile.Grass,
            (71, 142, 102) => GameMapTile.Grass,
            (71, 173, 115) => GameMapTile.Grass,
            (80, 134, 103) => GameMapTile.Grass,
            (81, 163, 117) => GameMapTile.Grass,
            (90, 154, 117) => GameMapTile.Grass,
            (118, 202, 126) => GameMapTile.Grass,
            (125, 195, 131) => GameMapTile.Grass,
            (132, 188, 137) => GameMapTile.Grass,
            _ => throw new InvalidOperationException($"Unknown color: {r}, {g}, {b}")
        };
    }

    public static void Subscribe(MapChunkOnChange callback)
    {
        onChange += callback;
    }

    public static void Update()
    {
        // Do map related things.
    }

    public static void Load(string path)
    {
        var fileData = File.ReadAllBytes($"{path}/Map.png");
        texture = new Texture2D(2048, 2048);
        if (!texture.LoadImage(fileData))
        {
            throw new Exception();
        }
    }
}
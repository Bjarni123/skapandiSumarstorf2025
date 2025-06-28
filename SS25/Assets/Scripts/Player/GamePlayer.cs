using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class GamePlayer
{
    public static GameObject Player;
    public static Vector2 Position { get; private set; }

    private static Vector2 previousPosition;
    public delegate void OnPositionChange(Vector2 previous, Vector2 current);
    private static OnPositionChange onPositionChange;

    public static void Subscribe(OnPositionChange callback)
    {
        onPositionChange += callback;
    }

    public static void Update()
    {
        Position = Player.transform.position;
        if (previousPosition != Position)
        {
            onPositionChange?.Invoke(previousPosition, Position);
        }
        previousPosition = Position;
    }

    public static void Load(string path)
    {
        var fileText = File.ReadAllText($"{path}/Player.json");
        var playerFile = JsonConvert.DeserializeObject<PlayerFile>(fileText);
        Position = playerFile.Position;
    }
}
using System.IO;
using UnityEngine;

public static class GameSave
{
    public static string Loaded { get; set; }

    static GameSave()
    {
        Loaded = $"{Application.dataPath}/Resources/Maps/Example";
    }

    public delegate void OnSave(string path);
    private static OnSave onSave;

    public static void Subscribe(OnSave callback)
    {
        onSave += callback;
    }

    public static void Save(string path)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), "GameSave_" + System.Guid.NewGuid().ToString());
        try
        {
            Directory.CreateDirectory(tempPath);

            onSave?.Invoke(tempPath);

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            Directory.Move(tempPath, path);
        }
        catch
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
            throw;
        }
    }
}
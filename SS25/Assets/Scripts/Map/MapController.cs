using UnityEngine;

public class MapController : MonoBehaviour
{
    MapChunkController[,] mapChunkControllers;

    public void Start()
    {
        GameMap.Load(GameSave.Loaded);
        GameMap.Subscribe(OnChunkChange);

        mapChunkControllers = new MapChunkController[GameMap.Width / 8, GameMap.Height / 8];
        for (var y = 0; y < GameMap.Height / 8; y++)
        {
            for (var x = 0; x < GameMap.Width / 8; x++)
            {
                var chunk_go = new GameObject($"{x}x{y}");
                chunk_go.transform.parent = transform;
                chunk_go.transform.position = new Vector3(x, y, 0);
                var controller = chunk_go.AddComponent<MapChunkController>();
                mapChunkControllers[x, y] = controller;
            }
        }
    }

    public void Update()
    {
        GameMap.Update();
    }

    public void OnChunkChange(int x, int y)
    {
        mapChunkControllers[x, y].OnChange();
    }
}
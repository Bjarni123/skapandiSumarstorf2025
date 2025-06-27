using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public int ChunkIndexDespawnDistance = 4;
    public int ChunkIndexSpawnDistance = 2;


    List<MapChunkController> mapChunkControllers;

    public void Start()
    {
        GameMap.Load(GameSave.Loaded);
        GameMap.Subscribe(OnChunkChange);
        GamePlayer.Subscribe(OnPlayerPositionChange);
        GameSave.Subscribe(OnSave);

        mapChunkControllers = new List<MapChunkController>();
    }

    public void Update()
    {
        GameMap.Update();
    }

    public void OnChunkChange(Vector2Int index)
    {
        var chunkController = mapChunkControllers.Find(chunk => chunk.Index == index);
        if (chunkController == null) return;
        chunkController.OnChange();
    }

    public void OnPlayerPositionChange(Vector2 previous, Vector2 current)
    {
        var current_chunk_index = new Vector2Int((int)current.x / 8, (int)current.y / 8);
        mapChunkControllers.RemoveAll(chunk =>
        {
            if ((int)Vector2Int.Distance(chunk.Index, current_chunk_index) > ChunkIndexDespawnDistance)
            {
                Destroy(chunk.gameObject);
                return true;
            }
            return false;
        });
        for (var y = -ChunkIndexSpawnDistance; y <= ChunkIndexSpawnDistance; y++)
        {
            for (var x = -ChunkIndexSpawnDistance; x <= ChunkIndexSpawnDistance; x++)
            {
                var chunk_index = current_chunk_index + new Vector2Int(x, y);
                var chunkController = mapChunkControllers.Find(chunk => chunk.Index == chunk_index);
                if (chunkController != null) continue;
                var chunk_go = new GameObject($"{chunk_index.x}x{chunk_index.y}");
                chunk_go.transform.parent = transform;
                chunk_go.transform.position = new Vector3(chunk_index.x * 8, chunk_index.y * 8, 0);
                var controller = chunk_go.AddComponent<MapChunkController>();
                controller.Index = chunk_index;
                mapChunkControllers.Add(controller);
            }
        }
    }

    public void OnSave(string path)
    {
        Debug.LogWarning("TODO");
    }
}
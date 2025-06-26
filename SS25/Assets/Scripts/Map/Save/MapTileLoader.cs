using UnityEngine;

public class MapTileLoader : MonoBehaviour
{
    public MapFileTile Tile { get; set; }

    public void Start()
    {
        switch (Tile)
        {
            case MapFileTile.Grass:
                Instantiate(Resources.Load<GameObject>("Tiles/Grass"), transform);
                break;
            case MapFileTile.Sand:
                Instantiate(Resources.Load<GameObject>("Tiles/Sand"), transform);
                break;
            case MapFileTile.Water:
                Instantiate(Resources.Load<GameObject>("Tiles/Water"), transform);
                break;
            default:
                Debug.LogWarning($"Spawning None tile");
                Instantiate(Resources.Load<GameObject>("Tiles/None"), transform);
                break;
        }
    }

    public MapFileTile Save()
    {
        return Tile;
    }
}
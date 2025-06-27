using UnityEngine;

public class MapTileLoader : MonoBehaviour
{
    public GameMapTile Tile { get; set; }

    public void Start()
    {
        switch (Tile)
        {
            case GameMapTile.Grass:
                Instantiate(Resources.Load<GameObject>("Tiles/Grass"), transform);
                break;
            case GameMapTile.Sand:
                Instantiate(Resources.Load<GameObject>("Tiles/Sand"), transform);
                break;
            case GameMapTile.Water:
                Instantiate(Resources.Load<GameObject>("Tiles/Water"), transform);
                break;
            default:
                Debug.LogWarning($"Spawning None tile");
                Instantiate(Resources.Load<GameObject>("Tiles/None"), transform);
                break;
        }
    }

    public GameMapTile Save()
    {
        return Tile;
    }
}
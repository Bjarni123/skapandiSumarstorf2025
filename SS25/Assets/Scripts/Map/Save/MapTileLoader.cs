using UnityEngine;

public class MapTileLoader : MonoBehaviour
{
    public MapFileTile Tile { get; set; }

    public void Start()
    {
        switch (Tile) {
            case MapFileTile.Grass:
                Instantiate(Resources.Load<GameObject>("Tiles/Grass"), transform);
                break;
            case MapFileTile.Water:
                Instantiate(Resources.Load<GameObject>("Tiles/Water"), transform);
                break;
            default:
                Instantiate(Resources.Load<GameObject>("Tiles/None"), transform);
                break;
        }
    }
}
using UnityEngine;

public class MapChunkController : MonoBehaviour
{
    public Vector2Int Index { get; set; }

    public void Start()
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                var tile = GameMap.GetTile(Index.x * 8 + x, Index.y * 8 + y);
                var go = (tile) switch
                {
                    GameMapTile.Grass =>
                        Instantiate(Resources.Load<GameObject>("Tiles/Grass"), transform),
                    GameMapTile.Sand =>
                        Instantiate(Resources.Load<GameObject>("Tiles/Sand"), transform),
                    GameMapTile.Water =>
                        Instantiate(Resources.Load<GameObject>("Tiles/Water"), transform),
                    _ => Instantiate(Resources.Load<GameObject>("Tiles/None"), transform),
                };
                go.transform.localPosition = new Vector3(x, y, 0);
            }
        }
    }

    public void OnChange()
    {
        Debug.LogWarning("TODO");
    }
}
using UnityEngine;

public class MapTilesLoader : MonoBehaviour, ISaveable
{
    public void Start()
    {
        MapLoader.TilesLoader = this;
        Load(MapLoader.File);
    }

    public void Load(MapFile file)
    {
        for (var y = 0; y < MapLoader.File.Tiles.GetLength(0); y++)
        {
            for (var x = 0; x < MapLoader.File.Tiles.GetLength(1); x++)
            {
                GameObject tile_go = Instantiate(Resources.Load<GameObject>("Tiles/Tile"), transform);
                var tile_loader = tile_go.GetComponent<MapTileLoader>();
                tile_loader.Tile = MapLoader.File.Tiles[y, x];
                tile_go.transform.localPosition = new Vector3(x, y, 0);
                tile_go.name = $"{x}x{y}";
            }
        }
    }

    public void Save(MapFile file)
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            var tile = transform.GetChild(i);
            var tile_loader = tile.GetComponent<MapTileLoader>();
            var name_split = tile.name.Split('x');
            var x = int.Parse(name_split[0]);
            var y = int.Parse(name_split[1]);
            MapLoader.File.Tiles[y, x] = tile_loader.Save();
        }
    }
}
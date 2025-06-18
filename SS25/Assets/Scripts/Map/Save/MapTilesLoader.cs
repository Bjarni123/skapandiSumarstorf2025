using UnityEngine;

public class MapTilesLoader : MonoBehaviour, ISaveable
{
    public void Start()
    {
        MapLoader.TilesLoader = this;
        Load(MapLoader.File);
    }

    public void Load(MapFile file) {
        for (var y= 0; y < MapLoader.File.Tiles.Count; y++) {
            var row = MapLoader.File.Tiles[y];
            for (var x = 0; x < row.Count; x++) {
                GameObject tile_go = Instantiate(Resources.Load<GameObject>("Tiles/Tile"), transform);
                var tile_loader = tile_go.GetComponent<MapTileLoader>();
                tile_loader.Tile = row[x];
                tile_go.transform.localPosition = new Vector3(x, y, 0);
                tile_go.name = $"{x}x{y}";
            }
        }
    }

    public void Save(MapFile file) { }
}
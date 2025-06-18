using UnityEngine;

public class MapObjectsLoader : MonoBehaviour, ISaveable
{
    public void Start()
    {
        MapLoader.ObjectsLoader = this;
        Load(MapLoader.File);
    }

    public void Load(MapFile file) {
        Debug.Log("TODO");
    }

    public void Save(MapFile file) { }
}
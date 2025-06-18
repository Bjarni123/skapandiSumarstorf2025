using UnityEngine;

public class MapMonstersLoader : MonoBehaviour, ISaveable
{
    public void Start()
    {
        MapLoader.MonstersLoader = this;
        Load(MapLoader.File);
    }

    public void Load(MapFile file) {
        Debug.Log("TODO");
    }

    public void Save(MapFile file) { }
}
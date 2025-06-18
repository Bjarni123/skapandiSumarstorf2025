using UnityEngine;

public class MapPlayersLoader : MonoBehaviour, ISaveable
{
    public void Start()
    {
        MapLoader.PlayersLoader = this;
        Load(MapLoader.File);
    }

    public void Load(MapFile file) {
        Debug.Log("TODO");
    }

    public void Save(MapFile file) { }
}
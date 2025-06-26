using UnityEngine;

public class MapPlayersLoader : MonoBehaviour, ISaveable
{
    public void Start()
    {
        MapLoader.PlayersLoader = this;
        Load(MapLoader.File);
    }

    public void Load(MapFile file)
    {
        foreach (var (name, player) in MapLoader.File.Players)
        {
            GameObject player_go = Instantiate(Resources.Load<GameObject>("Player"), transform);
            var player_loader = player_go.GetComponent<MapPlayerLoader>();
            player_go.transform.localPosition = new Vector3(player.Position.x, player.Position.y, -1);
            player_go.name = name;
        }
    }

    public void Save(MapFile file)
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            var player = transform.GetChild(i);
            var player_loader = player.GetComponent<MapPlayerLoader>();
            file.Players.Add(player.name, player_loader.Save());
        }
    }
}
using UnityEngine;

public class MapMonstersLoader : MonoBehaviour, ISaveable
{
    public void Start()
    {
        MapLoader.MonstersLoader = this;
        Load(MapLoader.File);
    }

    public void Load(MapFile file)
    {
        foreach (var (name, monster) in MapLoader.File.Monsters)
        {
            GameObject monster_go = null;
            switch (monster.Type)
            {
                case MapFileMonsterType.BlackCircle:
                    monster_go = Instantiate(Resources.Load<GameObject>("Monsters/BlackCircle"), transform);
                    break;
                default:
                    Debug.LogWarning($"Spawning invalid monster");
                    continue;
            }
            var monster_loader = monster_go.GetComponent<MapMonsterLoader>();
            monster_loader.Monster = monster.Type;
            monster_go.transform.localPosition = new Vector3(monster.Position.x, monster.Position.y, -1);
            monster_go.name = name;
        }
    }

    public void Save(MapFile file)
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            var monster = transform.GetChild(i);
            var monster_loader = monster.GetComponent<MapMonsterLoader>();
            file.Monsters.Add(monster.name, monster_loader.Save());
        }
    }
}
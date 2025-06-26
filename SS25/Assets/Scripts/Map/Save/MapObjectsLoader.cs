using UnityEngine;

public class MapObjectsLoader : MonoBehaviour, ISaveable
{
    public void Start()
    {
        MapLoader.ObjectsLoader = this;
        Load(MapLoader.File);
    }

    public void Load(MapFile file)
    {
        foreach (var (name, obj) in MapLoader.File.Objects)
        {
            GameObject object_go = null;
            switch (obj.Type)
            {
                case MapFileObjectType.Coin:
                    object_go = Instantiate(Resources.Load<GameObject>("Objects/Coin"), transform);
                    break;
                default:
                    Debug.LogWarning($"Spawning invalid object");
                    continue;
            }
            var object_loader = object_go.GetComponent<MapObjectLoader>();
            object_loader.Object = obj.Type;
            object_go.transform.localPosition = new Vector3(obj.Position.x, obj.Position.y, -1);
            object_go.name = name;
        }
    }

    public void Save(MapFile file)
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            var obj = transform.GetChild(i);
            var object_loader = obj.GetComponent<MapObjectLoader>();
            file.Objects.Add(obj.name, object_loader.Save());
        }
    }
}
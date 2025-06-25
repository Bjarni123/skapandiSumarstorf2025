using UnityEngine;

public class MapObjectLoader : MonoBehaviour
{
    public MapFileObjectType Object { get; set; }

    public MapFileObject Save()
    {
        return new MapFileObject
        {
            Type = Object,
            Position = new Vector2(transform.position.x, transform.position.y),
        };
    }
}
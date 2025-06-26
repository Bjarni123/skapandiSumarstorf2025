using UnityEngine;

public class MapPlayerLoader : MonoBehaviour
{
    public MapFilePlayer Save()
    {
        return new MapFilePlayer
        {
            Position = new Vector2(transform.position.x, transform.position.y),
        };
    }
}
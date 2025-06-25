using UnityEngine;

public class MapMonsterLoader : MonoBehaviour
{
    public MapFileMonsterType Monster { get; set; }

    public MapFileMonster Save()
    {
        return new MapFileMonster
        {
            Type = Monster,
            Position = new Vector2(transform.position.x, transform.position.y),
        };
    }
}
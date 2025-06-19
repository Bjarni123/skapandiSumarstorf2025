using UnityEngine;

public class Collectable : MonoBehaviour
{
    public CollectableType type;
    void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player)
        {
            player.numGoldSeed++;
            Destroy(this.gameObject);
        }
    }
}

public enum CollectableType
{
    NONE,
    GOLD_SEED
}

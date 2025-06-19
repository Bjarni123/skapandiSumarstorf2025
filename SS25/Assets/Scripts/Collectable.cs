using UnityEngine;

public class Collectable : MonoBehaviour
{
    public CollectableType type;
    public Sprite icon;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player)
        {
            player.inventory.Add(this);

            InventoryUI ui = FindFirstObjectByType<InventoryUI>();
            if (ui != null)
            {
                ui.Setup();
            }

            Destroy(this.gameObject);
        }
    }
}

public enum CollectableType
{
    NONE,
    GOLD,
    FISH,
    BEEF
}

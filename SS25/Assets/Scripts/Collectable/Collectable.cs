using UnityEngine;

public class Collectable : MonoBehaviour
{
    public CollectableType type;
    public Sprite icon;

    public Rigidbody2D rb2d;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player)
        {
            player.inventory.Add(this);

            if (player.inventoryUI != null && player.inventoryUI.inventoryPanel.activeSelf)
            {
                player.inventoryUI.Refresh();
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

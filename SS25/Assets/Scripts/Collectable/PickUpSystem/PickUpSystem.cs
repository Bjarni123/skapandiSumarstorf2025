using Inventory.Model;
using UnityEngine;

public class PickUpSystem : MonoBehaviour
{
    [SerializeField]
    private InventorySO inventoryData;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Triggered by: {collision.gameObject.name}");
        Item item = collision.GetComponent<Item>();
        if (item != null)
        {
            Debug.Log("Item found, attempting pickup.");
            int reminder = inventoryData.AddItem(item.InventoryItem, item.Quantity);
            if (reminder == 0)
                item.DestroyItem();
            else
                item.Quantity = reminder;
        }
        else
        {
            Debug.Log("No Item component found on collided object.");
        }
    }
}

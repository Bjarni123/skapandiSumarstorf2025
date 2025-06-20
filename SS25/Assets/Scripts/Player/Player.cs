using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Inventory inventory;
    public InventoryUI inventoryUI;

    private void Awake()
    {
        inventory = new Inventory(21);
        inventoryUI = Object.FindFirstObjectByType<InventoryUI>();
    }

    public void DropItem(Collectable item)
    {
        Vector2 spawnOffset = Random.insideUnitCircle.normalized * 1.25f;
        Vector2 spawnLocation = (Vector2)transform.position + spawnOffset;

        Collectable droppedItem = Instantiate(item, spawnLocation, Quaternion.identity);

        droppedItem.rb2d.AddForce(spawnOffset * .2f, ForceMode2D.Impulse);
    }
}

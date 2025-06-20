using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityEngine.InputSystem;
using NUnit.Framework;



public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;

    public Player player;

    public List<SlotUI> slots = new List<SlotUI>();

    private SlotUI selectedSlot;

    void Start()
    {
        inventoryPanel.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if(!inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(true);
            Refresh();
        }
        else
        {
            inventoryPanel.SetActive(false);
        }
    }

    public void Refresh()
    {
        if(slots.Count == player.inventory.slots.Count)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if(player.inventory.slots[i].type != CollectableType.NONE)
                {
                    slots[i].SetItem(player.inventory.slots[i]);
                }
                else
                {
                    slots[i].SetEmpty();
                }
            }
        }
    }

    public void Remove(int slotID)
    {
        Collectable itemToDrop = GameManager.instance.itemManager.GetItemByType(
            player.inventory.slots[slotID].type);

        if (itemToDrop != null)
        {
            player.DropItem(itemToDrop);
            player.inventory.Remove(slotID);
            Refresh();
        }
    }

    public void OnSlotClicked(SlotUI slot)
    {
        // Unhighlight previous
        if (selectedSlot != null)
            selectedSlot.Highlight(false);

        // Highlight new
        selectedSlot = slot;
        selectedSlot.Highlight(true);
    }
}

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
        }
        else
        {
            inventoryPanel.SetActive(false);
        }
    }

    public void Setup()
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
}

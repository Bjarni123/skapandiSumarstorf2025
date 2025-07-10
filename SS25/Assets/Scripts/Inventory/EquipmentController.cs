using System;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Model;
using Inventory.UI;

public class EquipmentController : MonoBehaviour
{
    [SerializeField]
    private EquipmentSlotUI[] equipmentSlots; // Assign in inspector, order: Sword, Axe, Pickaxe, Helmet, Chestplate, Boots

    private Dictionary<EquipmentType, EquipmentSlotUI> slotMap;

    [SerializeField]
    private ItemActionPanel actionPanel;

    [SerializeField]
    private InventorySO inventoryData;

    private void Awake()
    {
        slotMap = new Dictionary<EquipmentType, EquipmentSlotUI>();
        foreach (var slot in equipmentSlots)
        {
            slotMap[slot.AcceptedType] = slot;
            slot.HideBorder();
            slot.OnRightMouseBtnClick += HandleEquipmentSlotRightClick;
        }
    }

    public bool Equip(EquippableItemsSO item, GameObject player, List<ItemParameter> itemState)
    {
        if (item == null)
            return false;

        if (slotMap == null)
            return false;

        if (!slotMap.TryGetValue(item.equipmentType, out var slotUI))
            return false;

        if (slotUI == null)
            return false;

        bool set = false;
        try
        {
            set = slotUI.SetItem(item);
        }

        catch (Exception ex)
        {
            Debug.LogError($"Equip: Exception in slotUI.SetItem: {ex}");
            return false;
        }

        // Set in UI again (if needed)
        try
        {
            slotUI.SetItem(item);
        }

        catch (Exception ex)
        {
            Debug.LogError($"Equip: Exception in slotUI.SetItem (second call): {ex}");
            return false;
        }

        // Equip on player
        var agentWeapon = player != null ? player.GetComponent<AgentWeapon>() : null;
        if (agentWeapon != null && (item.equipmentType == EquipmentType.Sword || item.equipmentType == EquipmentType.Axe || item.equipmentType == EquipmentType.Pickaxe))
        {
            try
            {
                agentWeapon.SetWeapon(item, itemState);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Equip: Exception in agentWeapon.SetWeapon: {ex}");
            }
        }
        return true;
    }

    private void HandleEquipmentSlotRightClick(EquipmentSlotUI slotUI)
    {
        if (slotUI.GetEquippedItem() == null)
            return;

        actionPanel.ClearActions();
        actionPanel.AddButton("Unequip", () => Unequip(slotUI));
        actionPanel.Toggle(true);
        actionPanel.transform.position = slotUI.transform.position;
    }

    private void Unequip(EquipmentSlotUI slotUI)
    {
        var item = slotUI.GetEquippedItem();
        if (item == null)
            return;

        inventoryData.AddItem(item, 1);

        slotUI.ClearItem();
        slotUI.HideBorder();
        actionPanel.Toggle(false);
    }
}
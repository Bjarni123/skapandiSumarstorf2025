using System;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Model;

public class EquipmentController : MonoBehaviour
{
    [SerializeField]
    private EquipmentSlotUI[] equipmentSlots; // Assign in inspector, order: Sword, Axe, Pickaxe, Helmet, Chestplate, Boots

    private Dictionary<EquipmentType, EquipmentSlotUI> slotMap;

    private void Awake()
    {
        // Build a lookup for quick access
        slotMap = new Dictionary<EquipmentType, EquipmentSlotUI>();
        foreach (var slot in equipmentSlots)
        {
            slotMap[slot.AcceptedType] = slot;
        }
    }

    public bool Equip(EquippableItemsSO item, GameObject player, List<ItemParameter> itemState)
    {
        Debug.Log("Equip: Start");
        if (item == null)
        {
            Debug.LogError("Equip: item is null!");
            return false;
        }
        if (slotMap == null)
        {
            Debug.LogError("Equip: slotMap is null!");
            return false;
        }
        if (!slotMap.TryGetValue(item.equipmentType, out var slotUI))
        {
            Debug.LogError($"Equip: No slot found for type {item.equipmentType}");
            return false;
        }
        if (slotUI == null)
        {
            Debug.LogError("Equip: slotUI is null!");
            return false;
        }
        Debug.Log("Equip: slotUI found");

        bool set = false;
        try
        {
            set = slotUI.SetItem(item);
            Debug.Log($"Equip: slotUI.SetItem returned {set}");
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

        Debug.Log("Equip: End");
        return true;
    }
}
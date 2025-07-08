using Inventory.Model;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour
{
    [SerializeField]
    private EquipmentType acceptedType;

    public EquipmentType AcceptedType => acceptedType;

    private EquippableItemsSO equippedItem;

    [SerializeField]
    private Image iconImage;

    public bool SetItem(EquippableItemsSO item)
    {
        if (iconImage == null)
        {
            Debug.LogError("EquipmentSlotUI: iconImage is not assigned!");
            return false;
        }

        if (item == null)
        {
            ClearItem();
            return true;
        }

        if (item.equipmentType != acceptedType)
        {
            Debug.LogWarning($"Can't equip {item.equipmentType} in {acceptedType} slot!");
            return false;
        }

        equippedItem = item;
        iconImage.sprite = item.ItemImage;
        iconImage.enabled = true;
        Debug.Log($"iconImage assigned: {iconImage != null}, sprite: {item.ItemImage}");
        return true;
    }

    public void ClearItem()
    {
        if (iconImage == null)
        {
            Debug.LogError("iconImage is not assigned in EquipmentSlotUI!");
            return;
        }
        equippedItem = null;
        iconImage.sprite = null;
        iconImage.enabled = false;
    }

    public EquippableItemsSO GetEquippedItem()
    {
        return equippedItem;
    }
}

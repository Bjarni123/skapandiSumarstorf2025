using Inventory.Model;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private EquipmentType acceptedType;

    public EquipmentType AcceptedType => acceptedType;

    private EquippableItemsSO equippedItem;

    [SerializeField]
    private Image iconImageEquip;

    [SerializeField]
    public Image borderImageEquip;

    [SerializeField]
    public Image iconImagePreview;

    public event Action<EquipmentSlotUI> OnRightMouseBtnClick;

    public void HideBorder()
    {
        iconImageEquip.enabled = false;
        borderImageEquip.enabled = false;
    }

    public bool SetItem(EquippableItemsSO item)
    {
        if (iconImageEquip == null)
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
        iconImageEquip.sprite = item.ItemImage;
        iconImageEquip.enabled = true;
        iconImagePreview.enabled = false;
        Debug.Log($"iconImage assigned: {iconImageEquip != null}, sprite: {item.ItemImage}");
        return true;
    }

    public void ClearItem()
    {
        if (iconImageEquip == null)
        {
            Debug.LogError("iconImage is not assigned in EquipmentSlotUI!");
            return;
        }
        equippedItem = null;
        iconImageEquip.sprite = null;
        iconImageEquip.enabled = false;
        iconImagePreview.enabled = true;
    }

    public EquippableItemsSO GetEquippedItem()
    {
        return equippedItem;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightMouseBtnClick?.Invoke(this);
        }
    }
}

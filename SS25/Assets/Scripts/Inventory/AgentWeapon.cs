using Inventory.Model;
using System.Collections.Generic;
using UnityEngine;

public class AgentWeapon : MonoBehaviour
{
    [SerializeField]
    private EquippableItemsSO weapon;

    [SerializeField]
    private EquippableItemsSO axe;
    [SerializeField]
    private List<ItemParameter> axeState;

    [SerializeField]
    private EquippableItemsSO pickaxe;
    [SerializeField]
    private List<ItemParameter> pickaxeState;

    [SerializeField]
    private InventorySO inventoryData;

    [SerializeField]
    private List<ItemParameter> parametersToModify, itemCurrentState;

    public void SetWeapon(EquippableItemsSO weaponItemSO, List<ItemParameter> itemState)
    {
        switch (weaponItemSO.equipmentType)
        {
            case EquipmentType.Axe:
                axe = weaponItemSO;
                axeState = new List<ItemParameter>(itemState);
                ModifyParameters(axeState);
                break;
            case EquipmentType.Pickaxe:
                pickaxe = weaponItemSO;
                pickaxeState = new List<ItemParameter>(itemState);
                ModifyParameters(pickaxeState);
                break;
            default:
                break;
        }
    }

    private void ModifyParameters(List<ItemParameter> itemState)
    {
        if (itemState == null) return;
        foreach (var parameter in parametersToModify)
        {
            int index = itemState.FindIndex(p => p.itemParameter == parameter.itemParameter);
            if (index >= 0)
            {
                float newValue = itemState[index].value + parameter.value;
                itemState[index] = new ItemParameter
                {
                    itemParameter = parameter.itemParameter,
                    value = newValue
                };
            }
        }
    }

    public void ClearWeapon(EquipmentType type)
    {
        switch (type)
        {
            case EquipmentType.Axe:
                axe = null;
                axeState = null;
                break;
            case EquipmentType.Pickaxe:
                pickaxe = null;
                pickaxeState = null;
                break;
            default:
                break;
        }
    }

    public EquippableItemsSO GetAxe()
    {
        return axe;
    }

    /*public void SetWeapon(EquippableItemsSO weaponItemSO, List<ItemParameter> itemState)
    {
        if (weapon != null && weapon != weaponItemSO)
        {
            inventoryData.AddItem(weapon, 1, itemCurrentState);
        }

        this.weapon = weaponItemSO;
        this.itemCurrentState = new List<ItemParameter>(itemState);
        ModifyParameters();
    }*/


    /*private void ModifyParameters()
    {
        foreach (var parameter in parametersToModify)
        {
            if (itemCurrentState.Contains(parameter))
            {
                int index = itemCurrentState.IndexOf(parameter);
                float newValue = itemCurrentState[index].value + parameter.value;
                itemCurrentState[index] = new ItemParameter
                {
                    itemParameter = parameter.itemParameter,
                    value = newValue
                };
            }
        }
    }*/
}

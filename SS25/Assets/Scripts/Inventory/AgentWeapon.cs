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
    private EquippableItemsSO helmet;
    [SerializeField]
    private List<ItemParameter> helmetState;

    [SerializeField]
    private EquippableItemsSO chestplate;
    [SerializeField]
    private List<ItemParameter> chestplateState;

    [SerializeField]
    private EquippableItemsSO boots;
    [SerializeField]
    private List<ItemParameter> bootsState;

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
            case EquipmentType.Helmet:
                helmet = weaponItemSO;
                helmetState = new List<ItemParameter>(itemState);
                break;
            case EquipmentType.Chestplate:
                chestplate = weaponItemSO;
                chestplateState = new List<ItemParameter>(itemState);
                break;
            case EquipmentType.Boots:
                boots = weaponItemSO;
                bootsState = new List<ItemParameter>(itemState);
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

    public float GetAxeParameter(string paramName)
    {
        if (axe == null || axeState == null) return 1f;

        foreach (var param in axeState)
        {
            if (param.itemParameter.ParameterName == paramName)
                return param.value;
        }
        return 1f; // default multiplier
    }

    public EquippableItemsSO GetAxe()
    {
        return axe;
    }

    public float GetPickaxeParameter(string paramName)
    {
        if (pickaxe == null || pickaxeState == null) return 1f;

        foreach (var param in pickaxeState)
        {
            if (param.itemParameter.ParameterName == paramName)
                return param.value;
        }
        return 1f; // default multiplier
    }

    public EquippableItemsSO GetPickaxe()
    {
        return pickaxe;
    }

    public float GetHelmetParameter(string paramName)
    {
        if (helmet == null || helmetState == null) return 0f;

        foreach (var param in helmetState)
        {
            if (param.itemParameter.ParameterName == paramName)
                return param.value;
        }
        return 1f; // default multiplier
    }

    public EquippableItemsSO GetHelmet()
    {
        return helmet;
    }

    public float GetChestplateParameter(string paramName)
    {
        if (chestplate == null || chestplateState == null) return 0f;
        foreach (var param in chestplateState)
        {
            if (param.itemParameter.ParameterName == paramName)
                return param.value;
        }
        return 1f; // default multiplier
    }

    public EquippableItemsSO GetChestplate()
    {
        return chestplate;
    }

    public float GetBootsParameter(string paramName)
    {
        if (boots == null || bootsState == null) return 0f;
        foreach (var param in bootsState)
        {
            if (param.itemParameter.ParameterName == paramName)
                return param.value;
        }
        return 1f; // default multiplier
    }

    public EquippableItemsSO GetBoots()
    {
        return boots;
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Model
{
    public enum EquipmentType
    {
        None,
        Sword,
        Axe,
        Pickaxe,
        Helmet,
        Chestplate,
        Boots
    }

    [CreateAssetMenu]
    public class EquippableItemsSO : ItemSO, IDestroyableItem, IItemAction
    {
        public string ActionName => "Equip";

        [field: SerializeField]
        public AudioClip actionSFX { get; private set; }

        public EquipmentType equipmentType;
        public List<ItemParameter> parameters = new List<ItemParameter>();

        public bool PerformAction(GameObject character, List<ItemParameter> itemState = null)
        {
            AgentWeapon weaponSystem = character.GetComponent<AgentWeapon>();
            if (weaponSystem != null)
            {
                weaponSystem.SetWeapon(this, itemState == null ? DefaultParametersList : itemState);
                return true;
            }
            return false;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class HarvestableSO : ItemSO, IDestroyableItem//, IItemAction
    {
        [SerializeField]
        private List<ModifierData> modifiersData = new List<ModifierData>();

        [field: SerializeField]
        public AudioClip actionSFX { get; private set; }

        //public string ActionName => "Harvest";

        public bool PerformAction(GameObject character, List<ItemParameter> itemState)
        {
            // Example: Apply modifiers to character
            foreach (var modifier in modifiersData)
            {
                modifier.statModifier?.AffectCharacter(character, modifier.value);
            }
            Debug.Log("Harvested item!");
            return true;
        }
        }
}
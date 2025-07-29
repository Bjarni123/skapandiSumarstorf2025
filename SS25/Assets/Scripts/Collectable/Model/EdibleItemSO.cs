using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class EdibleItemSO : ItemSO, IDestroyableItem, IItemAction
    {
        [SerializeField]
        private List<ModifierData> modifiersData = new List<ModifierData>();
        public string ActionName => "Consume";

        [field: SerializeField]
        public AudioClip actionSFX { get; private set;}

        public bool PerformAction(GameObject character, List<ItemParameter> itemState)
        {
            if (modifiersData == null || modifiersData.Count == 0)
            {
                Debug.LogWarning("No modifiers set on this edible item!");
                return false;
            }

            foreach (ModifierData data in modifiersData)
            {
                if (data?.statModifier == null)
                {
                    Debug.LogWarning("Missing statModifier in ModifierData.");
                    continue;
                }

                data.statModifier.AffectCharacter(character, data.value);
            }

            return true;
        }

    }

    [Serializable]
    public class ModifierData
    {
        public CharacterStatModifierSO statModifier;
        public float value;
    }
}
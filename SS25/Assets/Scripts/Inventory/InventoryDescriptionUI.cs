using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class InventoryDescriptionUI : MonoBehaviour
    {
        [SerializeField]
        private Image itemImage;
        [SerializeField]
        private TMP_Text title;
        [SerializeField]
        private TMP_Text description;

        public void Awake()
        {
            ResetDescription();
        }

        public void ResetDescription()
        {
            itemImage.gameObject.SetActive(false);
            title.text = "";
            description.text = "";
        }

        public void SetDescription(Sprite sprite, string itemName, string itemDescription)
        {
            Debug.Log($"SetDescription called with sprite: {sprite}, name: {itemName}");
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = sprite;
            title.text = itemName;
            description.text = itemDescription;
        }

        public void UpdateDescription(int itemIndex, Sprite itemImage, string itemName, string itemDescription)
        {
            SetDescription(itemImage, itemName, itemDescription);
        }
    }
}
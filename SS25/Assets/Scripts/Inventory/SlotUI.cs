using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotUI : MonoBehaviour
{
    public Image itemIcon;
    public TextMeshProUGUI quantityText;
    public GameObject backgroundPanel; // Background panel for quantity number
    public GameObject highlightBorder; // Clicking item in inv highlights
    public Inventory.Slot slotData;
    public InventoryUI inventoryUI;


    public void SetItem(Inventory.Slot slot)
    {
        if(slot != null)
        {
            itemIcon.sprite = slot.icon;
            itemIcon.color = new Color(1, 1, 1, 1);
            quantityText.text = slot.count.ToString();
            backgroundPanel.SetActive(true);
        }
    }

    public void SetEmpty()
    {
        itemIcon.sprite = null;
        itemIcon.color = new Color(1, 1, 1, 0);
        quantityText.text = "";
        backgroundPanel.SetActive(false);
    }

    void Start()
    {
        highlightBorder.SetActive(false);
    }

    // Add this method to control the highlight border
    public void Highlight(bool on)
    {
        if (highlightBorder != null)
        {
            highlightBorder.SetActive(on);
        }
    }

    // Call this from your UI button or event
    public void OnClick()
    {
        if (inventoryUI != null)
        {
            inventoryUI.OnSlotClicked(this);
        }
    }
}


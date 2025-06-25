using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private InventoryItemUI itemPrefab;

    [SerializeField]
    private RectTransform contentPanel;

    [SerializeField]
    private InventoryDescriptionUI itemDescription;

    [SerializeField]
    private MouseFollower mouseFollower;


    List<InventoryItemUI> listOfItemsUI = new List<InventoryItemUI>();

    public event Action<int> OnDescriptionRequested, OnItemActionRequested, OnStartDragging;

    public event Action<int, int> OnSwapItems;

    private int currentlyDraggedItemIndex = -1;
    private void Awake()
    {
        Hide();
        mouseFollower.Toggle(false);
        itemDescription.ResetDescription();
    }

    public void InitilizeInventoryUI(int inventorySize)
    {
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        listOfItemsUI.Clear();
        for (int i = 0; i < inventorySize; i++)
        {
            InventoryItemUI itemUI =
            Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            itemUI.transform.SetParent(contentPanel, false);
            listOfItemsUI.Add(itemUI);
            itemUI.OnItemClicked += HandleItemSelection;
            itemUI.OnItemBeginDrag += HandleBeginDrag;
            itemUI.OnItemDroppedOn += HandleSwap;
            itemUI.OnItemEndDrag += HandleEndDrag;
            itemUI.OnRightMouseBtnClick += HandleShowItemActions;
        }
    }

    public void UpdateData(int itemIndex, Sprite itemImage, int itemQuantity)
    {
        if (listOfItemsUI.Count > itemIndex)
        {
            listOfItemsUI[itemIndex].SetData(itemImage, itemQuantity);
        }
    }

    public void CreateDraggedItem(Sprite sprite, int quantity)
    {
        mouseFollower.Toggle(true);
        mouseFollower.SetData(sprite, quantity);
    }

    private void HandleItemSelection(InventoryItemUI inventoryUIItem)
    {
        int index = listOfItemsUI.IndexOf(inventoryUIItem);
        if (index == -1)
            return;
        OnDescriptionRequested?.Invoke(index);
    }

    private void HandleSwap(InventoryItemUI inventoryUIItem)
    {
        int index = listOfItemsUI.IndexOf(inventoryUIItem);
        if (index == -1)
        {
            return;
        }
        OnSwapItems?.Invoke(currentlyDraggedItemIndex, index);
    }

    private void ResetDraggedItem()
    {
        mouseFollower.Toggle(false);
        currentlyDraggedItemIndex = -1;
    }

    private void HandleBeginDrag(InventoryItemUI inventoryUIItem)
    {
        int index = listOfItemsUI.IndexOf(inventoryUIItem);
        if (index == -1)
            return;
        currentlyDraggedItemIndex = index;
        HandleItemSelection(inventoryUIItem);
        OnStartDragging?.Invoke(index);
    }

    private void HandleEndDrag(InventoryItemUI inventoryUIItem)
    {
        ResetDraggedItem();
    }

    private void HandleShowItemActions(InventoryItemUI inventoryUIItem)
    {
        
    }

    public void Show()
    {
        gameObject.SetActive(true);
        ResetSelection();
    }

    private void ResetSelection()
    {
        itemDescription.ResetDescription();
        DeselectAllItems();
    }

    private void DeselectAllItems()
    {
        foreach (InventoryItemUI item in listOfItemsUI)
        {
            item.Deselect();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        ResetDraggedItem();
    }
}
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

    public Sprite image;//, image2;
    public int quantity;
    public string title, description;

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
        for (int i = 0; i < 24; i++)
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

    private void HandleItemSelection(InventoryItemUI obj)
    {
        itemDescription.SetDescription(image, title, description);
        listOfItemsUI[0].Select();
    }

    private void HandleBeginDrag(InventoryItemUI obj)
    {
        mouseFollower.Toggle(true);
        mouseFollower.SetData(image, quantity);
    }

    private void HandleSwap(InventoryItemUI obj)
    {
        
    }

    private void HandleEndDrag(InventoryItemUI obj)
    {
        mouseFollower.Toggle(false);
    }

    private void HandleShowItemActions(InventoryItemUI obj)
    {
        
    }

    public void Show()
    {
        gameObject.SetActive(true);
        itemDescription.ResetDescription();

        listOfItemsUI[0].SetData(image, quantity);
        //listOfItemsUI[1].SetData(image2, quantity);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
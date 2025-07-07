using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inventory.UI
{
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

        [SerializeField]
        private ItemActionPanel actionPanel;

        [SerializeField]
        private RectTransform inventoryPanelRect1;

        public event Action<int> OnDropItemRequested;

        public void ConnectPlayer(InventoryController controller)
        {
            // Optionally store the reference if you need to call back to the controller
        }

        public void InitializeUI()
        {
            // Optionally initialize UI elements here
        }

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
                InventoryItemUI itemUI = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
                itemUI.transform.SetParent(contentPanel, false);
                listOfItemsUI.Add(itemUI);
                itemUI.OnItemClicked += HandleItemSelection;
                itemUI.OnItemBeginDrag += HandleBeginDrag;
                itemUI.OnItemDroppedOn += HandleSwap;
                itemUI.OnItemEndDrag += HandleEndDrag;
                itemUI.OnRightMouseBtnClick += HandleShowItemActions;
            }
        }

        internal void ResetAllItems()
        {
            foreach (var item in listOfItemsUI)
            {
                item.ResetData();
                item.Deselect();
            }
        }

        internal void UpdateDescription(int itemIndex, Sprite itemImage, string Name, string description)
        {
            itemDescription.SetDescription(itemImage, Name, description);
            DeselectAllItems();
            listOfItemsUI[itemIndex].Select();
        }

        public void UpdateData(int itemIndex,
            Sprite itemImage, int itemQuantity)
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
            DeselectAllItems();
            listOfItemsUI[index].Select();
            OnDescriptionRequested?.Invoke(index);
        }
        
        private void HandleShowItemActions(InventoryItemUI inventoryUIItem)
        {
            int index = listOfItemsUI.IndexOf(inventoryUIItem);
            if (index == -1)
            {
                return;
            }
            DeselectAllItems();
            listOfItemsUI[index].Select();
            OnItemActionRequested?.Invoke(index);
        }

        private void HandleSwap(InventoryItemUI inventoryUIItem)
        {
            int index = listOfItemsUI.IndexOf(inventoryUIItem);
            if (index < 0 || currentlyDraggedItemIndex < 0)
            {
                return;
            }
            OnSwapItems?.Invoke(currentlyDraggedItemIndex, index);
            HandleItemSelection(inventoryUIItem);
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

        private void HandleEndDrag(InventoryItemUI inventoryUIItem, PointerEventData eventData)
        {
            Vector2 mousePosition = eventData.position;

            if (!RectTransformUtility.RectangleContainsScreenPoint(inventoryPanelRect1, mousePosition, eventData.enterEventCamera))
            {
                int index = listOfItemsUI.IndexOf(inventoryUIItem);
                if (index != -1)
                {
                    OnDropItemRequested?.Invoke(index);
                }
            }
            ResetDraggedItem();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            ResetSelection();
        }

        public void ResetSelection()
        {
            itemDescription.ResetDescription();
            DeselectAllItems();
        }

        public void AddAction(string actionName, Action performAction)
        {
            actionPanel.AddButton(actionName, performAction);
        }

        public void SetActions(List<(string, Action)> actions)
        {
            actionPanel.RemoveOldButtons();
            foreach (var (name, callback) in actions)
            {
                actionPanel.AddButton(name, callback);
            }
        }

        public void ShowItemAction(int itemIndex)
        {
            actionPanel.Toggle(true);
            actionPanel.transform.position = listOfItemsUI[itemIndex].transform.position;
        }

        private void DeselectAllItems()
        {
            foreach (InventoryItemUI item in listOfItemsUI)
            {
                item.Deselect();
            }
            actionPanel.Toggle(false);
        }

        public void ClearActions()
        {
            actionPanel.ClearActions();
        }

        public void Hide()
        {
            actionPanel.Toggle(false);
            gameObject.SetActive(false);
            ResetDraggedItem();
        }
    }
}
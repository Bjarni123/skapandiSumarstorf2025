using Inventory.Model;
using Inventory.UI;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Inventory
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField]
        private GameObject inventoryUIPrefab;

        [SerializeField]
        private InventoryUI inventoryUI;

        [SerializeField]
        private InventorySO inventoryData;

        public List<InventoryItem> initialItems = new List<InventoryItem>();

        [SerializeField]
        private AudioClip dropClip;

        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private EquipmentController equipmentController;

        private void Start()
        {
            // Instantiate the canvas
            var canvasObj = Instantiate(inventoryUIPrefab);

            // Find and assign InventoryUI
            if (inventoryUI == null)
            {
                inventoryUI = canvasObj.GetComponentInChildren<InventoryUI>(true);
                inventoryUI.ConnectPlayer(this);
            }

            // Find and assign EquipmentController
            if (equipmentController == null)
            {
                equipmentController = canvasObj.GetComponentInChildren<EquipmentController>(true);
                if (equipmentController == null)
                {
                    Debug.LogError("EquipmentController not found in instantiated InventoryCanvas!");
                }
            }

            inventoryUI.InitializeUI();
            PrepareUI();
            PrepareInventoryData();
        }

        private void PrepareInventoryData()
        {
            inventoryData.Initialize();
            inventoryData.OnInventoryUpdated += UpdateInventoryUI;
            foreach (InventoryItem item in initialItems)
            {
                if (item.IsEmpty)
                    continue;
                inventoryData.AddItem(item);
            }
        }

        private void UpdateInventoryUI(Dictionary<int, InventoryItem> inventoryState)
        {
            inventoryUI.ResetAllItems();
            foreach (var item in inventoryState)
            {
                inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage, item.Value.quantity);
            }
        }

        private void PrepareUI()
        {
            inventoryUI.InitilizeInventoryUI(inventoryData.Size);
            inventoryUI.OnDescriptionRequested += HandleDescriptionRequest;
            inventoryUI.OnSwapItems += HandleSwapItems;
            inventoryUI.OnStartDragging += HandleDragging;
            inventoryUI.OnItemActionRequested += HandleItemActionRequest;
            inventoryUI.OnDropItemRequested += HandleDropItemRequest;
        }

        private void HandleDropItemRequest(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (!inventoryItem.IsEmpty)
            {
                DropItem(itemIndex, inventoryItem.quantity);
            }
        }

        private void HandleDescriptionRequest(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
            {
                //inventoryUI.ResetSelection();
                return;
            }
            ItemSO item = inventoryItem.item;
            string description = PrepareDescription(inventoryItem);
            inventoryUI.UpdateDescription(itemIndex, item.ItemImage, item.Name, item.Description);
        }

        private string PrepareDescription(InventoryItem inventoryItem)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(inventoryItem.item.Description);
            sb.AppendLine();
            for (int i = 0; i < inventoryItem.itemState.Count; i++)
            {
                sb.Append($"{inventoryItem.itemState[i].itemParameter.ParameterName} : {inventoryItem.itemState[i].value} / {inventoryItem.item.DefaultParametersList[i].value}");
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private void HandleSwapItems(int itemIndex1, int itemIndex2)
        {
            inventoryData.SwapItems(itemIndex1, itemIndex2);
        }

        private void HandleDragging(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;
            inventoryUI.CreateDraggedItem(inventoryItem.item.ItemImage, inventoryItem.quantity);
        }

        private void HandleItemActionRequest(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;
            
            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null)
            {
                inventoryUI.ShowItemAction(itemIndex);
                inventoryUI.AddAction(itemAction.ActionName, () => PerformAction(itemIndex));
            }
            
            IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
            if (destroyableItem != null)
            {
                inventoryUI.SetActions(new List<(string, Action)>
                {
                    (itemAction.ActionName, () => PerformAction(itemIndex)),
                    ("Drop", () => ShowDropOptions(itemIndex, inventoryItem.quantity))
                });

            }
        }

        private void ShowDropOptions(int itemIndex, int quantity)
        {
            inventoryUI.ClearActions();

            inventoryUI.SetActions(new List<(string, Action)>
            {
                ("Drop 1", () => DropItem(itemIndex, 1)),
                ("Drop All", () => DropItem(itemIndex, quantity))
            });

            inventoryUI.ShowItemAction(itemIndex);
        }

        private void DropItem(int itemIndex, int quantity)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            SpawnDroppedItem(inventoryItem.item, quantity);

            inventoryData.RemoveItem(itemIndex, quantity);
            inventoryUI.ResetSelection();
            // audioSource.PlayOneShot(dropClip);
        }

        private void SpawnDroppedItem(ItemSO item, int quantity)
        {
            Vector3 dropPosition = transform.position;
            float randX = Random.value < 0.5f ? -1.5f : 1.5f;
            float randY = Random.value < 0.5f ? -1.5f : 1.5f;

            Vector3 spawnOffset = new Vector3(randX, randY, 0f).normalized * 1.5f;

            GameObject droppedItem = Instantiate(item.WorldPrefab, dropPosition + spawnOffset, Quaternion.identity);

            Item itemComponent = droppedItem.GetComponent<Item>();
            itemComponent.Initialize(item, quantity);

        }

        public void PerformAction(int itemIndex)
        {
            try
            {
                Debug.Log($"PerformAction called with itemIndex={itemIndex}");
                InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
                if (inventoryItem.IsEmpty)
                {
                    Debug.Log("inventoryItem.IsEmpty, returning early.");
                    return;
                }
                if (inventoryItem.item == null)
                {
                    Debug.LogError($"inventoryItem.item is null at index {itemIndex}");
                    return;
                }

                var equippable = inventoryItem.item as EquippableItemsSO;
                Debug.Log("Checked equippable");
                if (equippable != null)
                {
                    Debug.Log("Equippable found, calling Equip");
                    bool equipped = equipmentController.Equip(equippable, gameObject, inventoryItem.itemState);
                    Debug.Log("Equip called");
                    if (equipped)
                    {
                        inventoryData.RemoveItem(itemIndex, 1);
                        inventoryUI.ResetSelection();
                    }
                    return;
                }

                IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
                Debug.Log("Checked destroyableItem");
                if (destroyableItem != null)
                {
                    inventoryData.RemoveItem(itemIndex, 1);
                }

                IItemAction itemAction = inventoryItem.item as IItemAction;
                Debug.Log("Checked itemAction");
                if (itemAction != null)
                {
                    var state = inventoryItem.itemState ?? new List<ItemParameter>();
                    Debug.Log("About to call itemAction.PerformAction");
                    try
                    {
                        itemAction.PerformAction(gameObject, state);
                        Debug.Log("itemAction.PerformAction called");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Exception in PerformAction (inner catch): {ex}");
                    }

                    if (inventoryData.GetItemAt(itemIndex).IsEmpty)
                    {
                        inventoryUI.ResetSelection();
                    }
                }
                Debug.Log("End of PerformAction");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception in PerformAction (outer catch): {ex}");
            }
        }

        public void Update()
        {
            if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                if (inventoryUI.isActiveAndEnabled == false)
                {
                    inventoryUI.Show();
                    foreach (var item in inventoryData.GetCurrentInventoryState())
                    {
                        inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage, item.Value.quantity);
                    }
                }
                else
                {
                    inventoryUI.Hide();
                }
            }
        }
    }
}
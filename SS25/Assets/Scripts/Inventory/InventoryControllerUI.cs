using UnityEngine;
using UnityEngine.InputSystem;


public class InventoryController : MonoBehaviour
{
    [SerializeField]
    private InventoryUI inventoryUI;

    public int inventorySize = 24;

    private void Start()
    {
        inventoryUI.InitilizeInventoryUI(inventorySize);
    }

    public void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (inventoryUI.isActiveAndEnabled == false)
            {
                inventoryUI.Show();
            }
            else
            {
                inventoryUI.Hide();
            }
        }
    }
}

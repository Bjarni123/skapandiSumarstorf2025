using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField]
    private GameObject outlineSprite;

    private bool isPlayerInside = false;

    protected virtual void Start()
    {
        if (outlineSprite != null)
            outlineSprite.SetActive(false);
        else
            Debug.LogWarning($"{gameObject.name} is missing its outline sprite!");
    }

    private void Update()
    {
        if (isPlayerInside && Mouse.current.rightButton.wasPressedThisFrame)
        {
            Interact(); // Calls the overridden method in TreeInteractable
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            SetOutline(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            SetOutline(false);
        }
    }


    private void SetOutline(bool on)
    {
        if (outlineSprite != null)
            outlineSprite.SetActive(on);
    }

    public bool IsPlayerInRange() => isPlayerInside;

    public abstract void Interact();


}

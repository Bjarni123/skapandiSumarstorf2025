using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField]
    private GameObject outlineSprite;

    public bool isPlayerInRange = false;

    protected bool isChopping = false;

    protected virtual void Start()
    {
        if (outlineSprite != null)
            outlineSprite.SetActive(false);
        else
            Debug.LogWarning($"{gameObject.name} is missing its outline sprite!");
    }

    private void Update()
    {
        if (isPlayerInRange && Mouse.current.rightButton.wasPressedThisFrame)
        {
            Interact(); // Calls the overridden method in TreeInteractable
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            SetOutline(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;

            if (isChopping)
            {
                CancelChop();
            }

            SetOutline(false);
        }
    }


    private void SetOutline(bool on)
    {
        if (outlineSprite != null)
            outlineSprite.SetActive(on);
    }

    public bool IsPlayerInRange() => isPlayerInRange;

    public abstract void Interact();

    protected virtual void CancelChop() { }

}

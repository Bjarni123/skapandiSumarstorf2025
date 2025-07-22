using UnityEngine;

public class TreeInteractable : MonoBehaviour
{
    public SpriteRenderer highlightRenderer;
    public float chopRange = 1.5f;
    public Behaviour outlineEffect; // Reference to the Outline component

    private Transform player;
    private Animator playerAnimator;
    private bool playerInRange = false;
    private GameObject playerObj;

    private void Start()
    {
        highlightRenderer.enabled = false; // Start with highlight off

        if (outlineEffect != null)
            outlineEffect.enabled = false; // Start with outline off
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            highlightRenderer.enabled = true;
            playerInRange = true;
            playerObj = other.gameObject;

            if (outlineEffect != null)
            {
                outlineEffect.enabled = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            highlightRenderer.enabled = false;
            playerInRange = false;
            playerObj = null;

            if (outlineEffect != null)
            {
                outlineEffect.enabled = false;
            }
        }
    }

    public void TryChop()
    {
        if (playerInRange && playerObj != null)
        {
            var agentWeapon = playerObj.GetComponent<AgentWeapon>();
            if (agentWeapon != null && agentWeapon.GetAxe() != null)
            {
                Debug.Log("Chopping tree with axe!");
                // Chop logic here
            }
            else
            {
                Debug.Log("You need an axe to chop this tree.");
            }
        }
    }

    public void Chop(GameObject playerObj)
    {
        var agentWeapon = playerObj.GetComponent<AgentWeapon>();
        if (agentWeapon != null && agentWeapon.GetAxe() != null)
        {
            Debug.Log("Chopping tree with axe!");
            // Chop logic here
        }
        else
        {
            Debug.Log("You need an axe to chop this tree.");
        }
    }
}
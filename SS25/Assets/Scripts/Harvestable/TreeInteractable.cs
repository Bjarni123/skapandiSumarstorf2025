using UnityEngine;

public class TreeInteractable : MonoBehaviour
{
    public SpriteRenderer highlightRenderer;
    public float chopRange = 1.5f;

    private Transform player;
    private Animator playerAnimator;
    private bool playerInRange = false;
    private GameObject playerObj;

    private void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        if (player != null)
            playerAnimator = player.GetComponent<Animator>();
    }

    private void Update()
    {
        if (player == null || playerAnimator == null)
        {
            highlightRenderer.enabled = false;
            return;
        }

        float dist = Vector2.Distance(player.position, transform.position);
        if (dist <= chopRange && IsPlayerFacingTree())
        {
            highlightRenderer.enabled = true;
            // Optionally: Register this as the "active" interactable for the player
        }
        else
        {
            highlightRenderer.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            highlightRenderer.enabled = true;
            playerInRange = true;
            playerObj = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            highlightRenderer.enabled = false;
            playerInRange = false;
            playerObj = null;
        }
    }

    public bool CanChop()
    {
        if (player == null || playerAnimator == null)
            return false;

        float dist = Vector2.Distance(player.position, transform.position);
        return dist <= chopRange && IsPlayerFacingTree();
    }

    private bool IsPlayerFacingTree()
    {
        // Example: Use MoveX/MoveY as facing direction
        float moveX = playerAnimator.GetFloat("MoveX");
        float moveY = playerAnimator.GetFloat("MoveY");
        Vector2 facing = new Vector2(moveX, moveY).normalized;
        Vector2 toTree = ((Vector2)transform.position - (Vector2)player.position).normalized;

        // Consider a dot product threshold for "facing"
        return Vector2.Dot(facing, toTree) > 0.7f;
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
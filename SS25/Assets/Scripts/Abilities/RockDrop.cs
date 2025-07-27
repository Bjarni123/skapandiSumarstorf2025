using System.Collections;
using UnityEngine;

public class RockDrop : MonoBehaviour
{
    [Header("Timing")]
    public float delayBeforeFall = 1.5f;
    public float fallDuration = 0.4f;

    [Header("Damage")]
    public float damageRadius = 1.0f;
    public int damageAmount = 50;

    [Header("References")]
    public GameObject groundIndicator;
    public GameObject rockVisual;
    public GameObject permanentRockPrefab; // Prefab to instantiate on impact
    public LayerMask playerLayer;

    private Vector3 spawnOffset = new Vector3(0, 10f, 0); // Rock starts "above" the ground
    private Vector3 groundPosition;

    private void Start()
    {
        groundPosition = transform.position;
        rockVisual.SetActive(false);
        StartCoroutine(HandleRockDrop());
    }

    private IEnumerator HandleRockDrop()
    {
        // Step 1: Show indicator and wait
        groundIndicator.SetActive(true);
        yield return new WaitForSeconds(delayBeforeFall);

        // Step 2: Begin fall
        groundIndicator.SetActive(false);
        rockVisual.SetActive(true);
        rockVisual.transform.position = groundPosition + spawnOffset;

        float elapsed = 0f;
        while (elapsed < fallDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fallDuration;
            rockVisual.transform.position = Vector3.Lerp(groundPosition + spawnOffset, groundPosition, t);
            yield return null;
        }

        rockVisual.transform.position = groundPosition;

        // Step 3: Damage player
        Collider2D hit = Physics2D.OverlapCircle(groundPosition, damageRadius, playerLayer);
        if (hit != null)
        {
            // Assuming the player has a TakeDamage() method
            hit.GetComponent<PlayerHealth>()?.TakeDamage(damageAmount);
        }

        // Step 4: Place permanent obstacle
        Instantiate(permanentRockPrefab, groundPosition, Quaternion.identity);

        // Step 5: Clean up the falling rock
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}

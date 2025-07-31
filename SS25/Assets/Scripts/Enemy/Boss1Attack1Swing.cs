using System.Collections.Generic;
using UnityEngine;

public class Boss1Attack1Swing : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damage = 30;
    public float lifetime = 0.5f; // Lifetime before self-destruction if it doesn't hit

    private bool hasHit = false;

    void Start()
    {
        // Auto-destroy after swing ends
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return; // Prevent double-hit

        if (other.CompareTag("Player"))
        {
            // Call the player's damage function
            var health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            hasHit = true;
            Destroy(gameObject); // Destroy after hitting
        }
    }
}

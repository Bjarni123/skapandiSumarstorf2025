using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 5;
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime); // destroy after 3 sec
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth CurrentHealth = other.GetComponent<PlayerHealth>();
            if (CurrentHealth != null)
            {
                CurrentHealth.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}

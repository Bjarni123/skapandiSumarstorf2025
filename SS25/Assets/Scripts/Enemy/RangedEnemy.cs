using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    public Transform player;
    public float MoveSpeed = 5f;
    public float DesiredDistance = 8f;
    public float RetreatDistance = 4f;

    public GameObject projectilePrefab;
    public float _fireRate = 1f;
    private float _lastFireTime;

    private Rigidbody2D rb;
    public Transform FirePoint; // Changed from 'object' to 'Transform'

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position);
        float distance = direction.magnitude;
        direction.Normalize();

        if (distance > DesiredDistance)
        {
            rb.MovePosition(rb.position + direction * MoveSpeed * Time.deltaTime);
        }
        else if (distance < RetreatDistance)
        {
            rb.MovePosition(rb.position - direction * MoveSpeed * Time.deltaTime);
        }

        // Fire when in range and cooldown is ready
        if (distance <= DesiredDistance && distance >= RetreatDistance)
        {
            if (Time.time >= _lastFireTime + _fireRate)
            {
                Shoot(direction);
                _lastFireTime = Time.time;
            }
        }
    }

    void Shoot(Vector2 direction)
    {
        GameObject projectile = Instantiate(projectilePrefab, FirePoint.position, Quaternion.identity);

        // Ignore collision with self (enemy)
        Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        // Shoot toward player
        Vector2 shootDirection = (player.position - FirePoint.position).normalized;
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        projectileRb.linearVelocity = shootDirection * 5f; // Updated to use velocity
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DesiredDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, RetreatDistance);
    }
}


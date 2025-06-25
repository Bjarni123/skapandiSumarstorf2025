using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    public Transform player;

    public float MoveSpeed = 3f;
    public float DesiredDistance = 8f;
    public float RetreatDistance = 4f;
    public float DetectionRadius = 10f;

    public GameObject projectilePrefab;
    public float FireRate = 1f;
    public float ReloadDuration = 2.5f;

    private float _lastFireTime;
    private bool _isReloading = false;

    private Rigidbody2D rb;
    public Transform FirePoint;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position);
        float distance = direction.magnitude;
        
        if (distance > DetectionRadius)
            return; // Do nothing if outside detection range

        direction.Normalize();
        UpdateFacingDirection(direction);

        if (!_isReloading)
        {
            if (distance > DesiredDistance)
            {
                rb.MovePosition(rb.position + direction * MoveSpeed * Time.fixedDeltaTime);
            }
            else if (distance < RetreatDistance)
            {
                rb.MovePosition(rb.position - direction * MoveSpeed * Time.fixedDeltaTime);
            }

            if (distance <= DesiredDistance && distance >= RetreatDistance)
            {
                if (Time.time >= _lastFireTime + FireRate)
                {
                    Shoot(direction);
                    _lastFireTime = Time.time;
                    StartCoroutine(ReloadPause());
                }

            }
        }
    }

    void Shoot(Vector2 direction)
    {
        GameObject projectile = Instantiate(projectilePrefab, FirePoint.position, Quaternion.identity);
        Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        Vector2 shootDirection = (player.position - FirePoint.position).normalized;
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        projectileRb.linearVelocity = shootDirection * 5f;
    }

    System.Collections.IEnumerator ReloadPause()
    {
        _isReloading = true;
        yield return new WaitForSeconds(ReloadDuration);
        _isReloading = false;
    }

    void UpdateFacingDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            transform.rotation = Quaternion.Euler(0, 0, direction.x > 0 ? 0 : 180); // Right or Left
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, direction.y > 0 ? 90 : -90); // Up or Down
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, DetectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DesiredDistance);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, RetreatDistance);
    }
}
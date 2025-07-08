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

    private Rigidbody2D _rb;  // Fixed: consistent naming
    public Transform FirePoint;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();  // Fixed: correct variable name
        
        // Auto-find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (player == null || _rb == null) return;  // Added _rb null check

        Vector2 direction = (player.position - transform.position);
        float distance = direction.magnitude;
        
        if (distance > DetectionRadius)
            return;

        direction.Normalize();
        UpdateFacingDirection(direction);

        if (!_isReloading)
        {
            if (distance > DesiredDistance)
            {
                _rb.linearVelocity = direction * MoveSpeed;
            }
            else if (distance < RetreatDistance)
            {
                _rb.linearVelocity = -direction * MoveSpeed / 2;
            }
            else
            {
                _rb.linearVelocity = Vector2.zero; // Stop when in optimal range
            }
        }

        // Shoot when in range
        if (distance <= DesiredDistance && distance >= RetreatDistance)
        {
            if (Time.time >= _lastFireTime + FireRate)
            {
            Shoot(direction);
            _lastFireTime = Time.time;
            StartCoroutine(ReloadPause());
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
        }
        }
    }

    void Shoot(Vector2 direction)
    {
        Vector3 shootFromPosition = transform.position;
        Vector3 playerWorldPos = player.position;
        
        Vector2 shootDirection = (playerWorldPos - shootFromPosition).normalized;
        
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        GameObject projectile = Instantiate(projectilePrefab, shootFromPosition, rotation);
        Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        projectileRb.linearVelocity = shootDirection * 5f;
        
        Debug.Log($"Shooting from: {shootFromPosition} towards: {playerWorldPos}");
        Debug.Log($"Shoot direction: {shootDirection}");
        Debug.DrawRay(shootFromPosition, shootDirection * 3f, Color.red, 2f);
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
            transform.rotation = Quaternion.Euler(0, 0, direction.x > 0 ? 0 : 180);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, direction.y > 0 ? 90 : -90);
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
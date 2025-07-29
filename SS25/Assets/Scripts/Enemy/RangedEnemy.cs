using UnityEngine;

public class RangedEnemy : MonoBehaviour, IEnemy
{
    public Transform player;

    public float MoveSpeed = 3f;
    public float DesiredDistance = 8f;
    public float RetreatDistance = 4f;
    public float DetectionRadius = 10f;
    public float MaxHealth = 100f;

    public GameObject projectilePrefab;
    public float FireRate = 1f;
    public float ReloadDuration = 2.5f;

    [Header("Knockback")]
    public float KnockbackForce = 5f;
    public float KnockbackDuration = 0.3f;

    private float _currentHealth;
    private float _lastFireTime;
    private bool _isReloading = false;
    private bool _isAttacking = false;  // New: track attack state
    private Vector2 _attackDirection;   // Store attack direction for animation event
    private bool _isKnockedBack = false;
    private Rigidbody2D _rb;
    private Animator _animator;  // New: animator reference
    public Transform FirePoint;
    private EnemyHealthBar _healthBar;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();  // New: get animator component
        _healthBar = GetComponentInChildren<EnemyHealthBar>();
        _healthBar.SetHealth(_currentHealth, MaxHealth);
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
        if (player == null || _rb == null) return;

        Vector2 direction = (player.position - transform.position);
        float distance = direction.magnitude;
        
        if (distance > DetectionRadius)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        direction.Normalize();
        UpdateFacingDirection(direction);

        // Only move if not reloading AND not attacking
        if (!_isReloading && !_isAttacking)
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
        else
        {
            // Stop movement during attack or reload
            _rb.linearVelocity = Vector2.zero;
        }

        // Shoot when in range and not currently attacking
        if (distance <= DesiredDistance && distance >= RetreatDistance && !_isAttacking && !_isReloading)
        {
            if (Time.time >= _lastFireTime + FireRate)
            {
                StartAttack(direction);
                _lastFireTime = Time.time;
            }
        }
    }

    void StartAttack(Vector2 direction)
    {
        _isAttacking = true;
        
        // Trigger the attack animation
        if (_animator != null)
        {
            _animator.SetTrigger("Attack");
        }
        
        // Start the attack sequence
        StartCoroutine(AttackSequence(direction));
    }

    System.Collections.IEnumerator AttackSequence(Vector2 direction)
    {
        // Store the direction for the animation event to use
        _attackDirection = direction;
        
        // Wait for the attack animation to complete
        yield return new WaitForSeconds(GetAttackAnimationDuration());
        
        // Attack is complete, start reload
        _isAttacking = false;
        StartCoroutine(ReloadPause());
    }
    public void TakeDamage(float damage, Vector2 knockbackDirection)
    {

        _currentHealth -= damage;
        _healthBar.SetHealth(_currentHealth, MaxHealth);

        // Apply knockback
        StartCoroutine(ApplyKnockback(knockbackDirection));

        // Trigger damage animation
        if (_animator != null)
        {
            _animator.SetTrigger("TakeDamage");
        }

        if (_currentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        // Trigger death animation
        if (_animator != null)
        {
            _animator.SetTrigger("Die");
        }
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Disable the enemy after a short delay to allow death animation to play
        Destroy(gameObject, 2f);
    }
    System.Collections.IEnumerator ApplyKnockback(Vector2 direction)
    {
        _isKnockedBack = true;
        _rb.linearVelocity = direction.normalized * KnockbackForce;
        yield return new WaitForSeconds(KnockbackDuration);

        float slowDownTime = 0.3f;
        float elapsedTime = 0f;
        Vector2 startVelocity = _rb.linearVelocity;

        while (elapsedTime < slowDownTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / slowDownTime;
            _rb.linearVelocity = Vector2.Lerp(startVelocity, Vector2.zero, t);
            yield return null;
        }

        _isKnockedBack = false;
    }

    float GetAttackAnimationDuration()
    {
        // Method 1: Return a fixed duration
        // return 0.5f; // Adjust based on your animation length
        
        // Method 2: Get duration from animator (more dynamic)
        if (_animator != null)
        {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Attack")) // Replace "Attack" with your actual animation state name
            {
                return stateInfo.length;
            }
        }
        
        // Fallback duration
        return 0.5f;
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
        // Remove automatic rotation - let blend tree handle direction
        // Set blend tree parameters for animation direction
        if (_animator != null)
        {
            _animator.SetFloat("FacingX", direction.x);
            _animator.SetFloat("FacingY", direction.y);
        }
    }

    // Animation Event method - call this from your animation at the firing moment
    public void FireProjectile()
    {
        Shoot(_attackDirection);
    }

    // Optional: Animation Event method - call this from your animation
    public void AttackAnimationComplete()
    {
        _isAttacking = false;
        StartCoroutine(ReloadPause());
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
using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    [Header("Target & Detection")]
    public Transform player;
    public float DetectionRange = 10f;
    public float AttackRange = 2f;
    public float KeepDistance = 1.5f; // Stay this far from player
    
    [Header("Movement")]
    public float MoveSpeed = 3f;
    // Removed RotationSpeed since we're not rotating anymore
    
    [Header("Attack")]
    public float AttackDamage = 10f;
    public float AttackCooldown = 2f;
    public float AttackDuration = 0.5f; // How long attack animation lasts
    
    [Header("Health")]
    public float MaxHealth = 50f;
    
    [Header("Knockback")]
    public float KnockbackForce = 5f;
    public float KnockbackDuration = 0.3f;
    
    // Private variables
    private float _currentHealth;
    private bool _isKnockedBack = false;
    private float _lastAttackTime;
    private bool _isAttacking = false;
    private Vector2 _targetPosition;
    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    
    // Direction tracking for 4-way movement
    private Vector2 _lastMoveDirection;
    private Vector2 _facingDirection;
    
    // States
    private enum EnemyState
    {
        Idle,
        Chasing,
        Attacking,
        Dead
    }
    private EnemyState _currentState = EnemyState.Idle;

    void Start()
    {
        // Get components
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Initialize
        _currentHealth = MaxHealth;
        _facingDirection = Vector2.down; // Default facing direction
        
        // Lock rotation to keep enemy upright
        if (_rb != null)
        {
            _rb.freezeRotation = true;
        }
       
        // Auto-find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null || _currentState == EnemyState.Dead) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // State machine
        switch (_currentState)
        {
            case EnemyState.Idle:
                if (distanceToPlayer <= DetectionRange)
                {
                    _currentState = EnemyState.Chasing;
                }
                break;
                
            case EnemyState.Chasing:
                if (distanceToPlayer > DetectionRange)
                {
                    _currentState = EnemyState.Idle;
                    StopMoving();
                }
                else if (distanceToPlayer <= AttackRange && !_isAttacking)
                {
                    if (Time.time >= _lastAttackTime + AttackCooldown)
                    {
                        StartAttack();
                    }
                }
                else
                {
                    ChasePlayer(distanceToPlayer);
                }
                break;
                
            case EnemyState.Attacking:
                // Attacking is handled by coroutine
                break;
        }
        
        // Update animations
        UpdateAnimations();
    }

    void ChasePlayer(float distanceToPlayer)
    {
        // Don't move if knocked back
        if (_isKnockedBack) return;
        
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        
        // Calculate target position (stay KeepDistance away from player)
        if (distanceToPlayer > KeepDistance)
        {
            _targetPosition = player.position - (Vector3)(directionToPlayer * KeepDistance);
            
            // Move towards target
            Vector2 moveDirection = (_targetPosition - (Vector2)transform.position).normalized;
            _rb.linearVelocity = moveDirection * MoveSpeed;
            
            // Update facing direction for 4-way movement
            UpdateFacingDirection(moveDirection);
        }
        else
        {
            // Too close, stop moving
            StopMoving();
        }
    }

    void UpdateFacingDirection(Vector2 moveDirection)
    {
        if (moveDirection.magnitude < 0.1f) return;
        
        // Store the last move direction for animation
        _lastMoveDirection = moveDirection;
        
        // Determine 4-way facing direction
        if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.y))
        {
            // Moving more horizontally
            _facingDirection = moveDirection.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            // Moving more vertically
            _facingDirection = moveDirection.y > 0 ? Vector2.up : Vector2.down;
        }
        
        // Optional: Flip sprite horizontally based on direction
        if (_spriteRenderer != null)
        {
            _spriteRenderer.flipX = _facingDirection.x < 0;
        }
    }

    void StartAttack()
    {
        _currentState = EnemyState.Attacking;
        _isAttacking = true;
        _lastAttackTime = Time.time;
        
        // Stop moving during attack
        StopMoving();
        
        // Face the player without rotating
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        UpdateFacingDirection(directionToPlayer);
        
        // Start attack coroutine
        StartCoroutine(AttackSequence());
    }

    System.Collections.IEnumerator AttackSequence()
    {
        // Trigger attack animation
        if (_animator != null)
        {
            _animator.SetTrigger("Attack");
        }
        
        // Wait for attack animation to complete
        yield return new WaitForSeconds(AttackDuration);
        
        // Check if player is still in range and deal damage
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= AttackRange)
        {
            // Try to damage player
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)AttackDamage);
            }
        }
        
        // Reset attack state
        _isAttacking = false;
        _currentState = EnemyState.Chasing;
    }

    void StopMoving()
    {
        _rb.linearVelocity = Vector2.zero;
    }

    void UpdateAnimations()
    {
        if (_animator == null) return;
        
        // Set animation parameters
        _animator.SetBool("IsMoving", _rb.linearVelocity.magnitude > 0.1f);
        _animator.SetBool("IsAttacking", _isAttacking);
        _animator.SetBool("IsKnockedBack", _isKnockedBack);
        _animator.SetFloat("MoveSpeed", _rb.linearVelocity.magnitude);
        _animator.SetFloat("Health", _currentHealth / MaxHealth); // Health percentage
        
        // Set movement direction for 4-directional animations
        if (_rb.linearVelocity.magnitude > 0.1f)
        {
            _animator.SetFloat("MoveX", _lastMoveDirection.x);
            _animator.SetFloat("MoveY", _lastMoveDirection.y);
        }
        else
        {
            // Use facing direction when idle
            _animator.SetFloat("MoveX", _facingDirection.x);
            _animator.SetFloat("MoveY", _facingDirection.y);
        }
        
        // Additional parameters for 4-way directional animations
        _animator.SetFloat("FacingX", _facingDirection.x);
        _animator.SetFloat("FacingY", _facingDirection.y);
    }

    public void TakeDamage(float damage)
    {
        if (_currentState == EnemyState.Dead) return;
        
        _currentHealth -= damage;
        
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

    // Overloaded method to include knockback direction
    public void TakeDamage(float damage, Vector2 knockbackDirection)
    {
        if (_currentState == EnemyState.Dead) return;
        
        _currentHealth -= damage;
        
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

    System.Collections.IEnumerator ApplyKnockback(Vector2 direction)
    {
        _isKnockedBack = true;
        
        // Apply knockback force
        _rb.linearVelocity = direction.normalized * KnockbackForce;
        
        // Wait for knockback duration
        yield return new WaitForSeconds(KnockbackDuration);
        
        // Gradually slow down the knockback
        float elapsedTime = 0f;
        Vector2 startVelocity = _rb.linearVelocity;
        
        while (elapsedTime < 0.2f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 0.2f;
            _rb.linearVelocity = Vector2.Lerp(startVelocity, Vector2.zero, t);
            yield return null;
        }
        
        _rb.linearVelocity = Vector2.zero;
        _isKnockedBack = false;
    }

    void Die()
    {
        _currentState = EnemyState.Dead;
        StopMoving();
        
        if (_animator != null)
        {
            _animator.SetTrigger("Die");
        }
        
        // Disable collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }
        
        // Optional: Destroy after death animation
        Destroy(gameObject, 2f);
    }

    // Gizmos for debugging
    void OnDrawGizmosSelected()
    {
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectionRange);
        
        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
        
        // Keep distance
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, KeepDistance);
        
        // Show facing direction
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, _facingDirection * 1f);
    }
}
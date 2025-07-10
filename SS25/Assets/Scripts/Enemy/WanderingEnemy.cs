using UnityEngine;

public class WanderingEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float wanderRadius = 10f;
    public float wanderTimer = 5f;
    
    [Header("Detection Settings")]
    public float detectionRange = 8f;
    public LayerMask playerLayer = -1;
    public Transform player;
    
    [Header("Combat Settings")]
    public float shootRange = 6f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public int shotsToFire = 3;
    public float retreatDistance = 12f;
    
    [Header("Debug")]
    public bool showGizmos = true;
    
    private enum EnemyState
    {
        Wandering,
        Approaching,
        Shooting,
        Retreating
    }
    
    private EnemyState currentState = EnemyState.Wandering;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float timer;
    private float nextFireTime;
    private int shotsFired;
    private bool playerDetected;
    
    void Start()
    {
        startPosition = transform.position;
        SetNewWanderTarget();
        
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
        if (player == null) return;
        
        CheckPlayerDetection();
        HandleStateMachine();
        MoveTowardsTarget();
    }
    
    void CheckPlayerDetection()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Check if player is within detection range
        if (distanceToPlayer <= detectionRange)
        {
            // Optional: Add line of sight check
            if (HasLineOfSight())
            {
                playerDetected = true;
            }
        }
        else if (distanceToPlayer > retreatDistance)
        {
            playerDetected = false;
        }
    }
    
    bool HasLineOfSight()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, detectionRange);
        
        // If we hit something, check if it's the player
        if (hit.collider != null)
        {
            return hit.collider.CompareTag("Player");
        }
        
        // If we didn't hit anything, player is in sight
        return true;
    }
    
    void HandleStateMachine()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        switch (currentState)
        {
            case EnemyState.Wandering:
                HandleWandering();
                if (playerDetected)
                {
                    ChangeState(EnemyState.Approaching);
                }
                break;
                
            case EnemyState.Approaching:
                if (!playerDetected)
                {
                    ChangeState(EnemyState.Wandering);
                }
                else if (distanceToPlayer <= shootRange)
                {
                    ChangeState(EnemyState.Shooting);
                }
                else
                {
                    // Move towards player
                    targetPosition = player.position;
                }
                break;
                
            case EnemyState.Shooting:
                HandleShooting();
                if (shotsFired >= shotsToFire)
                {
                    ChangeState(EnemyState.Retreating);
                }
                break;
                
            case EnemyState.Retreating:
                if (distanceToPlayer >= retreatDistance)
                {
                    ChangeState(EnemyState.Wandering);
                }
                else
                {
                    // Move away from player
                    Vector3 retreatDirection = (transform.position - player.position).normalized;
                    targetPosition = transform.position + retreatDirection * 5f;
                }
                break;
        }
    }
    
    void HandleWandering()
    {
        timer += Time.deltaTime;
        
        // Check if we've reached the target or time to pick new target
        if (Vector3.Distance(transform.position, targetPosition) < 0.5f || timer >= wanderTimer)
        {
            SetNewWanderTarget();
        }
    }
    
    void HandleShooting()
    {
        // Stop moving while shooting
        targetPosition = transform.position;
        
        // Face the player while shooting
        Vector3 direction = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Adjust rotation based on your sprite orientation:
        // Use -90f if your sprite faces up, 0f if it faces right, 90f if it faces down
        transform.rotation = Quaternion.AngleAxis(angle + 180f, Vector3.forward);
        
        // Shoot at intervals
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
            shotsFired++;
        }
    }
    
    void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            
            // Add velocity to projectile if it has a Rigidbody2D
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector3 shootDirection = (player.position - firePoint.position).normalized;
                rb.linearVelocity = shootDirection * 10f; // Adjust speed as needed
            }
            
            Debug.Log($"Enemy fired shot {shotsFired + 1} of {shotsToFire}");
        }
        else
        {
            if (projectilePrefab == null) Debug.LogWarning("No projectile prefab assigned!");
            if (firePoint == null) Debug.LogWarning("No fire point assigned!");
        }
    }
    
    void SetNewWanderTarget()
    {
        timer = 0f;
        
        // Generate random point within wander radius
        Vector2 randomDirection = Random.insideUnitCircle * wanderRadius;
        targetPosition = startPosition + new Vector3(randomDirection.x, randomDirection.y, 0);
        
        // Make sure target is within bounds (optional)
        // You might want to add boundary checking here
    }
    
    void MoveTowardsTarget()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        
        // Optional: Rotate to face movement direction (for top-down)
        if (currentState != EnemyState.Shooting && direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // Adjust this based on your sprite orientation:
            // Use -90f if your sprite faces up by default
            // Use 0f if your sprite faces right by default
            // Use 90f if your sprite faces down by default
            transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        }
    }
    
    void ChangeState(EnemyState newState)
    {
        currentState = newState;
        
        // Reset state-specific variables
        switch (newState)
        {
            case EnemyState.Shooting:
                shotsFired = 0;
                nextFireTime = Time.time;
                break;
            case EnemyState.Wandering:
                SetNewWanderTarget();
                break;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Draw shoot range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRange);
        
        // Draw retreat distance
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, retreatDistance);
        
        // Draw wander radius from start position
        Gizmos.color = Color.green;
        if (Application.isPlaying)
            Gizmos.DrawWireSphere(startPosition, wanderRadius);
        else
            Gizmos.DrawWireSphere(transform.position, wanderRadius);
        
        // Draw current target
        Gizmos.color = Color.white;
        if (Application.isPlaying)
        {
            Gizmos.DrawWireSphere(targetPosition, 0.5f);
            Gizmos.DrawLine(transform.position, targetPosition);
        }
    }
}
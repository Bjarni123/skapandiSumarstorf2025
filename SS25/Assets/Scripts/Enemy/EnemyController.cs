using UnityEngine;

// Enhanced Enemy Controller that handles spawn point behavior
public class EnemyController : MonoBehaviour
{
    [Header("Spawn Point Behavior")]
    public float spawnPointRadius = 3f;
    public float returnToSpawnSpeed = 2f;
    public float playerDetectionRange = 10f;
    
    private Vector3 spawnPoint;
    private Transform player;
    private bool isReturningToSpawn = false;
    private bool playerInRange = false;
    
    // Reference to your existing enemy script components
    private RangedEnemy rangedEnemy;
    private Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rangedEnemy = GetComponent<RangedEnemy>();
        
        // Set spawn point to current position if not set
        if (spawnPoint == Vector3.zero)
        {
            spawnPoint = transform.position;
        }
    }
    
    void Update()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        playerInRange = distanceToPlayer <= playerDetectionRange;
        
        if (playerInRange)
        {
            // Player is in range - let the existing enemy AI take over
            isReturningToSpawn = false;
            
            // Enable the ranged enemy script
            if (rangedEnemy != null)
            {
                rangedEnemy.enabled = true;
            }
        }
        else
        {
            // Player is out of range - return to spawn point
            if (rangedEnemy != null)
            {
                rangedEnemy.enabled = false;
            }
            
            ReturnToSpawnPoint();
        }
    }
    
    void ReturnToSpawnPoint()
    {
        float distanceToSpawn = Vector3.Distance(transform.position, spawnPoint);
        
        if (distanceToSpawn > 0.5f)
        {
            isReturningToSpawn = true;
            
            // Move towards spawn point
            Vector3 direction = (spawnPoint - transform.position).normalized;
            rb.linearVelocity = direction * returnToSpawnSpeed;
            
            // Face the direction we're moving
            UpdateFacingDirection(direction);
        }
        else
        {
            // We're at spawn point, stop moving
            isReturningToSpawn = false;
            rb.linearVelocity = Vector2.zero;
        }
    }
    
    void UpdateFacingDirection(Vector2 direction)
    {
    if (direction.x > 0)
    {
        // Facing right
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.localScale = new Vector3(1, 1, 1);
    }
    else if (direction.x < 0)
    {
        // Facing left
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.localScale = new Vector3(-1, 1, 1);
    }
    }
    
    public void SetSpawnPoint(Vector3 point)
    {
        spawnPoint = point;
    }
    
    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
        
        // Also set the player reference for the ranged enemy
        if (rangedEnemy != null)
        {
            rangedEnemy.player = playerTransform;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw spawn point
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spawnPoint, spawnPointRadius);
        
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRange);
        
        // Draw line to spawn point
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, spawnPoint);
    }
    
    void OnDestroy()
    {
        // Notify the spawner that this enemy was destroyed
        EnemySpawner spawner = FindFirstObjectByType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.OnEnemyDestroyed(gameObject);
        }
    }

    public int health = 10;

    public void TakeDamage(int damageAmount)
    {
        Debug.Log("Take Damage");
        health -= damageAmount;
        if (health <= 0) Destroy(gameObject);
    }
}
using UnityEngine;

// Enhanced Enemy Controller that handles spawn point behavior
public class EnemyController : MonoBehaviour
{
    [Header("Spawn Point Behavior")]
    public float SpawnPointRadius = 3f;
    public float ReturnToSpawnSpeed = 2f;
    public float PlayerDetectionRange = 10f;
    
    private Vector3 spawnPoint;
    private Transform player;
    private bool isReturningToSpawn = false;
    private bool playerInRange = false;
    
    // Reference to your existing enemy script components
    private RangedEnemy _rangedEnemy;
    private Rigidbody2D _rb;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rangedEnemy = GetComponent<RangedEnemy>();
        
        // Ensure proper initial scale
        transform.localScale = new Vector3(1, 1, 1);
        transform.rotation = Quaternion.identity;
        
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
        playerInRange = distanceToPlayer <= PlayerDetectionRange;
        
        if (playerInRange)
        {
            // Player is in range - let the existing enemy AI take over
            isReturningToSpawn = false;
            
            // Enable the ranged enemy script
            if (_rangedEnemy != null)
            {
                _rangedEnemy.enabled = true;
            }
        }
        else
        {
            // Player is out of range - return to spawn point
            if (_rangedEnemy != null)
            {
                _rangedEnemy.enabled = false;
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
            _rb.linearVelocity = direction * ReturnToSpawnSpeed;
            
            // Face the direction we're moving
            UpdateFacingDirection(direction);
        }
        else
        {
            // We're at spawn point, stop moving
            isReturningToSpawn = false;
            _rb.linearVelocity = Vector2.zero;
        }
    }
    
    void UpdateFacingDirection(Vector3 direction)
    {
        // Get the current scale magnitude to preserve size
        float currentScale = transform.localScale.x;
        float scaleSize = Mathf.Abs(currentScale);
        
        // For 2D sprites, usually you only want to flip horizontally
        if (direction.x > 0)
        {
            // Facing right - normal orientation
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
        }
        else if (direction.x < 0)
        {
            // Facing left - flip horizontally
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.localScale = new Vector3(-scaleSize, scaleSize, scaleSize);
        }
        // Don't rotate for vertical movement, just keep current horizontal facing
    }
    
    public void SetSpawnPoint(Vector3 point)
    {
        spawnPoint = point;
    }
    
    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
        
        // Also set the player reference for the ranged enemy
        if (_rangedEnemy != null)
        {
            _rangedEnemy.player = playerTransform;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw spawn point
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spawnPoint, SpawnPointRadius);
        
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, PlayerDetectionRange);
        
        // Draw line to spawn point
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, spawnPoint);
    }
    
    void OnDestroy()
    {
        // Notify the spawner that this enemy was destroyed
        EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.OnEnemyDestroyed(gameObject);
        }
    }
}
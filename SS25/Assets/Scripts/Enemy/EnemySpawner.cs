using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject EnemyPrefab;
    public int MaxEnemies = 12;
    public float SpawnRadius = 20f;
    public float MinDistanceFromPlayer = 5f;
    public float MaxDistanceFromPlayer = 15f;
    
    [Header("Spawn Area")]
    public Vector2 SpawnAreaMin = new Vector2(-50, -50);
    public Vector2 SpawnAreaMax = new Vector2(50, 50);
    
    [Header("Cluster Settings")]
    public int MinEnemiesPerCluster = 2;
    public int MaxEnemiesPerCluster = 4;
    public float ClusterRadius = 2f;
    public float MinClusterDistance = 5f;  // Minimum distance between clusters
    
    [Header("Respawn Settings")]
    public float RespawnDelay = 5f;
    public bool RespawnEnemies = true;
    
    private Transform _player;
    private List<GameObject> _spawnedEnemies = new List<GameObject>();
    private List<Vector3> _clusterCenters = new List<Vector3>();
    
    void Start()
    {
        // Find the player
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        if (_player == null)
        {
            Debug.LogError("Player not found! Make sure player has 'Player' tag.");
            return;
        }
        
        // Initial spawn
        SpawnInitialEnemies();
    }
    
    void Update()
    {
        // Check if we need to respawn enemies
        if (RespawnEnemies && _spawnedEnemies.Count < MaxEnemies)
        {
            // Respawn in clusters when enemies are destroyed
            int enemiesToSpawn = Mathf.Min(Random.Range(MinEnemiesPerCluster, MaxEnemiesPerCluster + 1), 
                                         MaxEnemies - _spawnedEnemies.Count);
            SpawnCluster(enemiesToSpawn);
        }
        
        // Clean up destroyed enemies from the list
        CleanupDestroyedEnemies();
    }
    
    void SpawnInitialEnemies()
    {
        int enemiesSpawned = 0;
        int maxClusters = Mathf.CeilToInt((float)MaxEnemies / MaxEnemiesPerCluster);
        
        for (int i = 0; i < maxClusters && enemiesSpawned < MaxEnemies; i++)
        {
            int enemiesToSpawn = Mathf.Min(Random.Range(MinEnemiesPerCluster, MaxEnemiesPerCluster + 1), 
                                         MaxEnemies - enemiesSpawned);
            
            SpawnCluster(enemiesToSpawn);
            enemiesSpawned += enemiesToSpawn;
        }
    }
    
    void SpawnCluster(int enemyCount)
    {
        Vector3 clusterCenter = GetValidClusterCenter();
        
        if (clusterCenter == Vector3.zero)
        {
            Debug.LogWarning("Could not find valid cluster center");
            return;
        }
        
        _clusterCenters.Add(clusterCenter);
        
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPosition = GetPositionInCluster(clusterCenter);
            SpawnEnemyAtPosition(spawnPosition);
        }
    }
    
    Vector3 GetValidClusterCenter()
    {
        int attempts = 0;
        int maxAttempts = 50;
        
        while (attempts < maxAttempts)
        {
            // Generate random position within spawn area
            float x = Random.Range(SpawnAreaMin.x, SpawnAreaMax.x);
            float y = Random.Range(SpawnAreaMin.y, SpawnAreaMax.y);
            Vector3 position = new Vector3(x, y, 0);
            
            // Check distance from player
            float distanceFromPlayer = Vector3.Distance(position, _player.position);
            
            if (distanceFromPlayer >= MinDistanceFromPlayer && distanceFromPlayer <= MaxDistanceFromPlayer)
            {
                // Check if position is not too close to other cluster centers
                bool validPosition = true;
                foreach (Vector3 existingCenter in _clusterCenters)
                {
                    if (Vector3.Distance(position, existingCenter) < MinClusterDistance)
                    {
                        validPosition = false;
                        break;
                    }
                }
                
                if (validPosition)
                {
                    return position;
                }
            }
            
            attempts++;
        }
        
        return Vector3.zero;
    }
    
    Vector3 GetPositionInCluster(Vector3 clusterCenter)
    {
        // Generate random position within cluster radius
        Vector2 randomOffset = Random.insideUnitCircle * ClusterRadius;
        return clusterCenter + new Vector3(randomOffset.x, randomOffset.y, 0);
    }
    
    void SpawnEnemyAtPosition(Vector3 position)
    {
        GameObject enemy = Instantiate(EnemyPrefab, position, Quaternion.identity);
        
        // Set up the enemy with its spawn point
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.SetSpawnPoint(position);
            enemyController.SetPlayer(_player);
        }
        
        // Add to tracking list
        _spawnedEnemies.Add(enemy);
    }
    
    void CleanupDestroyedEnemies()
    {
        for (int i = _spawnedEnemies.Count - 1; i >= 0; i--)
        {
            if (_spawnedEnemies[i] == null)
            {
                _spawnedEnemies.RemoveAt(i);
            }
        }
    }
    
    // Call this when an enemy is destroyed
    public void OnEnemyDestroyed(GameObject enemy)
    {
        int index = _spawnedEnemies.IndexOf(enemy);
        if (index >= 0)
        {
            _spawnedEnemies.RemoveAt(index);
        }
        
        // Don't immediately respawn individual enemies
        // Let the Update method handle cluster respawning
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw spawn area
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3((SpawnAreaMin.x + SpawnAreaMax.x) / 2, (SpawnAreaMin.y + SpawnAreaMax.y) / 2, 0);
        Vector3 size = new Vector3(SpawnAreaMax.x - SpawnAreaMin.x, SpawnAreaMax.y - SpawnAreaMin.y, 0);
        Gizmos.DrawWireCube(center, size);
        
        // Draw cluster centers and radii
        Gizmos.color = Color.red;
        foreach (Vector3 clusterCenter in _clusterCenters)
        {
            Gizmos.DrawWireSphere(clusterCenter, ClusterRadius);
            Gizmos.DrawSphere(clusterCenter, 0.2f); // Small dot for center
        }
        
        // Draw individual enemy positions
        Gizmos.color = Color.orange;
        foreach (GameObject enemy in _spawnedEnemies)
        {
            if (enemy != null)
            {
                Gizmos.DrawWireSphere(enemy.transform.position, 0.3f);
            }
        }
        
        // Draw distance ranges from player
        if (_player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_player.position, MinDistanceFromPlayer);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_player.position, MaxDistanceFromPlayer);
        }
    }
}
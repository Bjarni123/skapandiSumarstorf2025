using UnityEngine;
using System.Collections.Generic;

public class TrollCamp : MonoBehaviour
{
    [Header("Spawning")]
    public GameObject trollPrefab;
    public int maxTrolls = 3;
    public float spawnRadius = 5f;
    public float spawnInterval = 10f;
    
    [Header("Detection")]
    public float detectionRange = 8f;
    public LayerMask playerLayer;
    
    private List<GameObject> spawnedTrolls = new List<GameObject>();
    private bool playerNearby = false;
    
    void Start()
    {
        InvokeRepeating("CheckSpawning", 1f, spawnInterval);
    }
    
    void Update()
    {
        // Check if player is nearby
        playerNearby = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        
        // Clean up destroyed trolls from list
        spawnedTrolls.RemoveAll(troll => troll == null);
    }
    
    void CheckSpawning()
    {
        if (playerNearby && spawnedTrolls.Count < maxTrolls)
        {
            SpawnTroll();
        }
    }
    
    void SpawnTroll()
    {
        Vector3 spawnPos = transform.position + (Vector3)(Random.insideUnitCircle * spawnRadius);
        GameObject newTroll = Instantiate(trollPrefab, spawnPos, Quaternion.identity);
        spawnedTrolls.Add(newTroll);
    }
}
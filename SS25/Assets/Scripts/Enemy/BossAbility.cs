using UnityEngine;

public class BossAbility : MonoBehaviour
{
    public GameObject rockPrefab;

    [SerializeField]
    float abilityCD = 10f;
    [SerializeField]
    float abilityRadius = 10f;
    [SerializeField]
    int numberOfRocks = 3;

    private float lastAbilityTime;
    private Transform position;

    private void Start()
    {
        position = GetComponent<Transform>();
    }



    void RockAbility()
    {
        for (int i = 0; i < numberOfRocks; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * abilityRadius;
            Vector2 bossPos = position.position;
            Vector2 spawnPosition = bossPos + randomOffset;

            Instantiate(rockPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, abilityRadius);
    }

}

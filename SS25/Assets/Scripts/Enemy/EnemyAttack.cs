using UnityEngine;

public class EnemyAttack : MonoBehaviour
{

    public Transform player;
    public float AttackRange = 1.5f;
    public float AttackCooldown = 1f;
    public int AttackDamage = 1;

    private float _lastAttackTime;

    void Start()
    {

    }


    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= AttackRange && Time.time >= _lastAttackTime + AttackCooldown)
        {
            Attack();
            _lastAttackTime = Time.time;
        }
    }

    void Attack()
    {
        Debug.Log("Enemy attacks player for " + AttackDamage + " damage!");

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(AttackDamage);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}

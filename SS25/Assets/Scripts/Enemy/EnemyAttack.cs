using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public Transform player;
    public float AttackRange = 1.5f;
    public float AttackCooldown = 1f;
    public int AttackDamage = 1;

    [HideInInspector] public bool IsAttacking = false;

    private float _lastAttackTime;

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= AttackRange && Time.time >= _lastAttackTime + AttackCooldown)
        {
            StartCoroutine(Attack());
            _lastAttackTime = Time.time;
        }
    }

    System.Collections.IEnumerator Attack()
    {
        IsAttacking = true;

        Debug.Log("Enemy attacks player for " + AttackDamage + " damage!");

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(AttackDamage);
        }

        // Wait a short time before allowing movement again
        yield return new WaitForSeconds(0.4f); // you can adjust this
        IsAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}

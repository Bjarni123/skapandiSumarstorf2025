using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 2;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TriggerEnter");
        var enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
            enemy.TakeDamage(damage);
    }
}

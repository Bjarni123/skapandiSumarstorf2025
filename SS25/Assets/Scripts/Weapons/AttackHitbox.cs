using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public float duration = 0.2f;
    public int damage = 10;

    void Start() => Destroy(gameObject, duration);

    void OnTriggerEnter2D(Collider2D other)
    {
        var enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
            enemy.TakeDamage(damage);
    }
}

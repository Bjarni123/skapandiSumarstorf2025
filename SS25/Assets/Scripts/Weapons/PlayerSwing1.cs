using System.Collections.Generic;
using UnityEngine;

public class PlayerSwing1 : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] LayerMask enemyLayer = default;

    HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    [HideInInspector] public float damage = 1f;
    [HideInInspector] public Vector2 knockBackDir = Vector2.zero;

    void Reset()
    {
        // Make sure your collider is a trigger
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    public void Initialize(float damage, Vector2 knockbackDir)
    {
        this.damage = damage;
        this.knockBackDir = knockbackDir;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 1) Layer check—to only hit things on your “Enemy” layer
        if ((enemyLayer.value & (1 << other.gameObject.layer)) == 0)
            return;

        var go = other.gameObject;

        // 2) Single‐hit guard
        if (hitEnemies.Contains(go))
            return;

        // 3) Grab the concrete EnemyHealth (or whatever script has TakeDamage)
        var enemyHealth = go.GetComponent<SimpleEnemy>();
        if (enemyHealth != null)
        {
            Debug.Log("Enemy should take dmg");
            enemyHealth.TakeDamage(damage, knockBackDir);
            hitEnemies.Add(go);
        }
        else
        {
            Debug.LogWarning($"Hitbox collided with {go.name} but no EnemyHealth found.");
        }
    }

}

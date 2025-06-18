using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int health = 10;

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0) Destroy(gameObject);
    }
}

using UnityEngine;
public interface IEnemy
{
    void TakeDamage(float damage, Vector2 knockbackDir);
}
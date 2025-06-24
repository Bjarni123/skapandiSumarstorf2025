using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChase : MonoBehaviour
{
    public Transform player;
    public float DetectionRadius = 4f;
    public float MoveSpeed = 2f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (player == null || (GetComponent<EnemyAttack>().IsAttacking)) return;

        Vector2 enemyPos = rb.position;
        Vector2 playerPos = player.position;

        Vector2 direction = (playerPos - enemyPos).normalized;
        float distance = Vector2.Distance(enemyPos, playerPos);

        if (distance < DetectionRadius)
        {
            Vector2 newPosition = enemyPos + direction * MoveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);

            // Flip sprite to face direction
            if (direction.x != 0)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Sign(direction.x) * Mathf.Abs(scale.x);
                transform.localScale = scale;
            }
        }
    }
}

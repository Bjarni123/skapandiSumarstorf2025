using Unity.Burst.Intrinsics;
using UnityEngine;

public class BossBehaviour : MonoBehaviour
{

    [Header("Ability")]
    [SerializeField]
    GameObject rockPrefab;
    [SerializeField]
    float abilityCD = 10f;
    [SerializeField]
    float abilityRadius = 10f;
    [SerializeField]
    int numberOfRocks = 3;

    private float lastAbilityTime;

    // Attributes 
    private Transform tf;
    private Animator anim;
    private Rigidbody2D rb;

    [Header("Attacking")]
    [SerializeField]
    float attackRange = 2f;
    public Transform player;
    public float DetectionRadius = 4f;

    [Header("Movement")]
    public float MoveSpeed = 2f;
    private Vector2 MovementDirection;
    private float distanceToPlayer;

    [Header("Health")]
    [SerializeField] int maxHealth = 200;
    [SerializeField] int currentHealth = 0;


    private void Start()
    {
        tf = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        currentHealth = maxHealth;
    }
    

    private void Update()
    {

        UpdateVariables();

        if (!CanSeePlayer() || IsPerformingAction())
        {
            return;
        }

        if (CanAttackPlayer())
        {
            Attack();
        }
        else if (CanUseAbility())
        {
            anim.Play("Boss1_Attack2");
        }
    }

    private void FixedUpdate()
    {
        if (IsPerformingAction() || !CanSeePlayer())
        {
            return;
        }
        
        ChasePlayer();
    }

    void UpdateVariables()
    {
        Vector2 bossPos = tf.position;
        Vector2 playerPos = player.position;

        distanceToPlayer = Vector2.Distance(bossPos, playerPos);
        if (distanceToPlayer < DetectionRadius)
        {
            MovementDirection = (playerPos - bossPos).normalized;
        }
        else
        {
            MovementDirection = Vector2.zero;
        }

        anim.SetFloat("MoveMagnitude", MovementDirection.x + MovementDirection.y);
    }

    void Attack()
    {
        if (Random.Range(1, 3) == 1)
        {
            anim.Play("Boss1_Attack1");
        }
        else
        {
            anim.Play("Boss1_Attack3");
        }
    }

    void ChasePlayer()
    {

        Vector2 bossPos = tf.position;
        Vector2 newPosition = bossPos + MovementDirection * MoveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);

        if (MovementDirection.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(MovementDirection.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    bool CanSeePlayer()
    {
        if (distanceToPlayer < DetectionRadius) { return true; }

        else { return false; }
    }

    bool CanAttackPlayer()
    {
        if (distanceToPlayer < attackRange) { return true; } 
        else { return false; }
    }

    bool CanUseAbility()
    {
        return (Time.time - lastAbilityTime > abilityCD); 
    }

    public void TakeDamage(int dmg_amount)
    {
        currentHealth -= dmg_amount;
    }

    public void TakeDamage(int dmg_amount, Vector2 knockbackDir)
    {
        TakeDamage(dmg_amount);
        TakeKnockback(knockbackDir);
    }
    
    private void TakeKnockback(Vector2 knockbackDir)
    {
        return;
    }


    private void Die()
    {
        return;
    }

    public void RockAbility()
    {
        lastAbilityTime = Time.time;

        for (int i = 0; i < numberOfRocks; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * abilityRadius;
            Vector2 bossPos = tf.position;
            Vector2 spawnPosition = bossPos + randomOffset;

            Instantiate(rockPrefab, spawnPosition, Quaternion.identity);
        }
    }

    public bool IsPerformingAction()
    {
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

        return !(state.IsName("Boss1_Idle") || state.IsName("Boss1_Walk"));
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, abilityRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, DetectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}

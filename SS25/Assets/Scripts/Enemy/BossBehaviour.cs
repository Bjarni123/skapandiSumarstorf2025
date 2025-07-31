using System.Collections;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class BossBehaviour : MonoBehaviour
{

    [Header("Ability")]
    [SerializeField] GameObject rockPrefab;
    [SerializeField] float abilityCD = 10f;
    [SerializeField] float abilityRadius = 10f;
    [SerializeField] int numberOfRocks = 3;

    private float lastAbilityTime;

    // Attributes 
    private Transform tf;
    private Animator anim;
    private Rigidbody2D rb;

    [Header("Attacking")]
    [SerializeField] GameObject Attack1Prefab;
    [SerializeField] GameObject Attack2Prefab;
    [SerializeField] float attackRange = 2f;
    [SerializeField] float attackCD = 1f;
    public Transform player;
    public float DetectionRadius = 4f;

    private float lastAttackTime;

    [Header("Movement")]
    public float MoveSpeed = 2f;
    private Vector2 MovementDirection;
    private float distanceToPlayer;

    [Header("Health")]
    [SerializeField] int maxHealth = 200;
    [SerializeField] float currentHealth = 0;

    [Header("Knockback")]
    [SerializeField] float knockbackForce = 1f;
    [SerializeField] float knockbackDuration = 0.5f;
    public bool isKnockedback = false;
    private Coroutine knockbackRoutine;


    [Header("Flash Settings")]
    [SerializeField] float flashDuration = 0.1f;
    private SpriteRenderer sr;
    private Color originalColor;
    private Coroutine flashRoutine;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("No SpriteRenderer found on " + gameObject.name);
        }
        originalColor = sr.color;
    }


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
            if (!isKnockedback)
            {
                rb.linearVelocity = Vector3.zero;
            }
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
        
        lastAttackTime = Time.time;
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
        if (distanceToPlayer < attackRange && Time.time - lastAttackTime > attackCD) { return true; } 
        else { return false; }
    }

    bool CanUseAbility()
    {
        return (Time.time - lastAbilityTime > abilityCD); 
    }

    public void TakeDamage(float dmg_amount)
    {
        currentHealth -= dmg_amount;
        Flash();
        // anim.Play("Boss1_TakeHit");

        if (currentHealth <= 0) { Die(); }
        else { anim.Play("Boss1_TakeHit"); }
    }

    public void TakeDamage(float dmg_amount, Vector2 knockbackDir)
    {
        TakeKnockback(knockbackDir);        
        TakeDamage(dmg_amount);

    }

    public void TakeKnockback(Vector2 knockbackDir)
    {
        if (knockbackRoutine != null)
        {
            StopCoroutine(knockbackRoutine);
        }

        knockbackRoutine = StartCoroutine(HandleKnockback(knockbackDir.normalized));
    }

    public void Flash()
    {

        // Cancel only the currently running flash coroutine
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator HandleKnockback(Vector2 dir)
    {
        isKnockedback = true;

        rb.linearVelocity = dir * knockbackForce;

        yield return new WaitForSeconds(knockbackDuration);

        rb.linearVelocity = Vector2.zero;
        isKnockedback = false;
    }

    private IEnumerator FlashRoutine()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        sr.color = originalColor;
    }


    private void Die()
    {
        Destroy(gameObject);
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

    public void SpawnAttack1Prefab()
    {
        // 2.5 0.8
        Vector2 currPos = rb.position;

        float xDir;
        if (MovementDirection.x > 0)
        {
            xDir = 2.5f;
        }
        else
        {
            xDir = -2.5f;
        }

        Vector2 spawnPos = currPos + new Vector2(xDir, 0.8f);
        Instantiate(Attack1Prefab, spawnPos, Quaternion.identity);
    }

    public void SpawnAttack2Prefab()
    {
        // 1.2 0.3
        Vector2 currPos = rb.position;
        float xDir;
        if (MovementDirection.x > 0)
        {
            xDir = 1.6f;
        }
        else
        {
            xDir = -1.6f;
        }

        Vector2 spawnPos = currPos + new Vector2(xDir, 0.5f);
        Instantiate(Attack2Prefab, spawnPos, Quaternion.identity);
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

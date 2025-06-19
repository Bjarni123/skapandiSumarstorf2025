using UnityEngine;
using UnityEngine.InputSystem;    // new Input System

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject attackPrefab;
    public float cooldown = 0.5f;
    public float spawnOffset = 1f;

    [Header("Animation")]
    public Animator weaponAnimator;


    float nextAttackTime = 0f;

    void Update()
    {
        // Left-click
        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + cooldown;
            // autoAttack();
            weaponAnimator.SetTrigger("AutoAttack");
        }
    }

    public void SpawnHitBox()
    {
        // compute dir & spawnPos exactly as before...
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorld.z = 0;
        Vector3 dir = (mouseWorld - transform.position).normalized;
        Vector3 spawnPos = transform.position + dir * spawnOffset;

        Instantiate(attackPrefab, spawnPos,
            Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg));
    }

    /*
    void autoAttack()
    {
        // 1. Mouse world position
        Vector3 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;

        // 2. Direction vector
        Vector3 dir = (mouseWorld - transform.position).normalized;

        // 3. Calculate spawn position
        Vector3 spawnPos = transform.position + dir * swordSwishSpawnOffset;

        // 4. Instantiate and rotate
 

        GameObject swish = Instantiate(attackPrefab, spawnPos, Quaternion.identity);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        swish.transform.rotation = Quaternion.Euler(0f, 0f, angle - 30);

    }
    */
}

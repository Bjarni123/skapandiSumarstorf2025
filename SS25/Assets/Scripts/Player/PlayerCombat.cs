using UnityEngine;
using UnityEngine.InputSystem;    // new Input System

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject attackPrefab;
    public float cooldown = 0.5f;
    public float swordSwishSpawnOffset = 1f;


    float nextAttackTime = 0f;

    void Update()
    {
        // Left-click
        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + cooldown;
            autoAttack();
        }
    }

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
        swish.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}

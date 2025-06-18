using UnityEngine;
using UnityEngine.InputSystem;    // new Input System

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject attackPrefab;
    public float cooldown = 0.5f;

    float nextAttackTime = 0f;

    void Update()
    {
        // Left-click
        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + cooldown;
            Instantiate(attackPrefab, transform.position, Quaternion.identity);
        }
    }
}

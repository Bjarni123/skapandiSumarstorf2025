using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.InputSystem.XR;
using Unity.VisualScripting;


/*
 * It is possible to switch direction mid combo, Good to decide if that's a feature we would like
 * 
 * 
 */

public class PlayerCombat : MonoBehaviour
{

    Animator anim;


    // These are experimental, their definition is not perfectly defined, adjusting needed when combat system is fully implemented
    [SerializeField] float comboCD = 0.2f;
    [SerializeField] float attackCD = 0.5f;
    [SerializeField] float nextComboAttackDelay = 0.6f;


    public List<AttackSO> combo;
    float lastClickedTime;
    float lastComboEnd;
    int comboCounter;

    private PlayerStateManager playerStateManager;


    [Header("Hitbox Settings")]
    [SerializeField] GameObject hitboxPrefab;
    [SerializeField] float spawnDistance = 1f;
    [SerializeField] float hitboxDuration = 0.2f;

    GameObject spawnedHitbox;

    private void Awake()
    {
        playerStateManager = GetComponent<PlayerStateManager>();
        if (playerStateManager == null)
        {
            Debug.LogError("PlayerStateManager component not found on the GameObject.");
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerStateManager.IsPerformingAction())
        {
            ProcessCombatInputs();    
        }
        ExitAttack();
    }

    private void ProcessCombatInputs()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Attack();
        }
    }

    void Attack()
    {
        if (Time.time - lastComboEnd > comboCD && comboCounter <= combo.Count)
        {
            CancelInvoke("EndCombo");

            if (Time.time - lastClickedTime >= attackCD)
            {
                anim.runtimeAnimatorController = combo[comboCounter].animatorOV;
                anim.Play("AutoAttack", 0, 0);
                comboCounter++;
                lastClickedTime = Time.time;


                if (comboCounter >= combo.Count)
                {
                    comboCounter = 0;
                }
            }
        }
    }

    void ExitAttack()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9 && anim.GetCurrentAnimatorStateInfo(0).IsTag("AutoAttack"))
        {
            Invoke("EndCombo", nextComboAttackDelay);
        }
    }

    void EndCombo()
    {
        comboCounter = 0;
        lastComboEnd = Time.time;
    }

    public void SpawnHitPrefab()
    {

        float x = anim.GetFloat("LastMoveX");
        float y = anim.GetFloat("LastMoveY");
        Vector2 dir = new Vector2(x, y);

        Vector3 worldPos = transform.position + (Vector3)dir * spawnDistance;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        float x_rot;

        if (x > 0)
        {
            x_rot = 0f;
        }
        else
        {
            x_rot = 180f;
        }

        Vector3 spawnOffset = Vector3.zero;

        if (x > 0)
        {
            spawnOffset = new Vector3(-0.5f, -0.8f, 0);
        }
        else if (x < 0)
        {
            spawnOffset = new Vector3(0.5f, -0.8f, 0);
        }
        else if (y > 0)
        {
            spawnOffset = new Vector3(-1f, -0.4f, 0);
        }
        else
        {
            spawnOffset = new Vector3(1f, 0.4f, 0);
        }



        Quaternion rot = Quaternion.Euler(x_rot, 180f, angle);

        GameObject swishPrefab = combo[comboCounter].swishPrefab;

        spawnedHitbox = Instantiate(swishPrefab, worldPos + spawnOffset, rot);
        PlayerSwing1 hb = spawnedHitbox.GetComponent<PlayerSwing1>();

        float damage = combo[comboCounter].damage;

        hb.Initialize(damage, dir);
    }
}

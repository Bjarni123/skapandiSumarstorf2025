using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;


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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessCombatInputs();
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
            Debug.Log(comboCounter);
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
}

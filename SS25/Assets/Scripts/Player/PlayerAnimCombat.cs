using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerAnimCombat : MonoBehaviour
{

    Animator anim;

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
    }

    private void ProcessCombatInputs()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Attack();
        }
        ExitAttack();
    }

    public void OnAttackFinished()
    {
        // not used rn
        // anim.SetBool("IsAttacking", false);
        // Debug.Log("OnAttackFinished");
    }

    void Attack()
    {
        if (Time.time - lastComboEnd > 0.5f && comboCounter <= combo.Count)
        {
            CancelInvoke("EndCombo");

            if (Time.time - lastClickedTime >= 0.2f)
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

        /*if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            anim.SetTrigger("AutoAttack");
            anim.SetBool("IsAttacking", true);
        }*/
    }

    void ExitAttack()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9 && anim.GetCurrentAnimatorStateInfo(0).IsTag("AutoAttack"))
        {
            Invoke("EndCombo", 1);
        }
    }

    void EndCombo()
    {
        comboCounter = 0;
        lastComboEnd = Time.time;
    }
}

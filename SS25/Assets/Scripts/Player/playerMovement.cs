using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;

    [Header("Rolling Settings")]
    public float rollSpeed = 50f;
    public float rollCooldown = 1f;

    private float lastRollTimer;

    Rigidbody2D rb;
    Animator anim;

    private Vector2 input;
    private Vector2 lastMoveDirection;

    private bool facingRight = true;


    private PlayerStateManager playerStateManager;
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        playerStateManager = GetComponent<PlayerStateManager>();
    }

    void Update()
    {
        if (!playerStateManager.IsPerformingAction())
        {
            ProcessMovementInputs();
        }
        Animate();
        HandleFlip();
    }

    private void FixedUpdate()
    {
        if (!playerStateManager.IsPerformingAction())
        {
            rb.linearVelocity = input * speed;
        }
        else if (playerStateManager.IsPerformingAttack())
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    
    private void ProcessMovementInputs()
    {
        
        /* 
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("AutoAttack"))
        {
            return;
        }
        */

        var kb = Keyboard.current;
        // read WASD or arrow keys
        float moveX = (kb.aKey.isPressed || kb.leftArrowKey.isPressed) ? -1f
                : (kb.dKey.isPressed || kb.rightArrowKey.isPressed) ? 1f
                : 0f;
        float moveY = (kb.sKey.isPressed || kb.downArrowKey.isPressed) ? -1f
                : (kb.wKey.isPressed || kb.upArrowKey.isPressed) ? 1f
                : 0f;

        input = new Vector2(moveX, moveY).normalized;

        if (input != Vector2.zero)
        {
            if (kb.leftShiftKey.isPressed && (Time.time - lastRollTimer >= rollCooldown)  )
            {
                StartRoll();
            } else {
                lastMoveDirection = input;
            }
        }
    }

    void Animate()
    {
        anim.SetFloat("MoveX", input.x);
        anim.SetFloat("MoveY", input.y);
        anim.SetFloat("MoveMagnitude", input.magnitude);
        anim.SetFloat("LastMoveX", lastMoveDirection.x);
        anim.SetFloat("LastMoveY", lastMoveDirection.y);
    }

    void HandleFlip()
    {
        if (input.x < 0 && facingRight || input.x > 0 && !facingRight)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            facingRight = !facingRight;
        }
    }

    void StartRoll()
    {
        anim.Play("PlayerRoll_Temp", 0, 0);
        rb.linearVelocity = input * rollSpeed;
        lastRollTimer = Time.time;
    }
}

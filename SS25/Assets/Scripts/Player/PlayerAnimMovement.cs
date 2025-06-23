using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimMovement : MonoBehaviour
{
    public float speed = 1f;
    Rigidbody2D rb;

    private Vector2 input;

    Animator anim;
    private Vector2 lastMoveDirection;
    private bool facingRight = true;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        ProcessInputs();
        Animate();

        if (input.x < 0 && facingRight || input.x > 0 && !facingRight)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = input * speed;
    }
    
    void ProcessInputs()
    {
        var kb = Keyboard.current;
        // read WASD or arrow keys
        float moveX = (kb.aKey.isPressed || kb.leftArrowKey.isPressed) ? -1f
                : (kb.dKey.isPressed || kb.rightArrowKey.isPressed) ? 1f
                : 0f;
        float moveY = (kb.sKey.isPressed || kb.downArrowKey.isPressed) ? -1f
                : (kb.wKey.isPressed || kb.upArrowKey.isPressed) ? 1f
                : 0f;

        input = new Vector2(moveX, moveY).normalized;

        // float moveX = Input.GetAxisRaw("Horizontal");
        // float moveY = Input.GetAxisRaw("Vertical");

        if ((moveX != 0f && moveY != 0f) && (input.x != 0 || input.y != 0))
        {
            lastMoveDirection = input;
        }

        // input.x = Input.GetAxisRaw("Horizontal");
        // input.y = Input.GetAxisRaw("Vertical");

        // input.Normalize();
    }

    void Animate()
    {
        anim.SetFloat("MoveX", input.x);
        anim.SetFloat("MoveY", input.y);
        anim.SetFloat("MoveMagnitude", input.magnitude);
        anim.SetFloat("LastMoveX", lastMoveDirection.x);
        anim.SetFloat("LastMoveY", lastMoveDirection.y);
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        facingRight = !facingRight;
    }
}

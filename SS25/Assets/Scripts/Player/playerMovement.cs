 using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    Rigidbody2D rb;
    Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // rb.freezeRotation = true;
    }

    void Update()
    {
        var kb = Keyboard.current;
        // read WASD or arrow keys
        float x = (kb.aKey.isPressed || kb.leftArrowKey.isPressed) ? -1f
                : (kb.dKey.isPressed || kb.rightArrowKey.isPressed) ? 1f
                : 0f;
        float y = (kb.sKey.isPressed || kb.downArrowKey.isPressed) ? -1f
                : (kb.wKey.isPressed || kb.upArrowKey.isPressed) ? 1f
                : 0f;

        moveInput = new Vector2(x, y).normalized;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
}

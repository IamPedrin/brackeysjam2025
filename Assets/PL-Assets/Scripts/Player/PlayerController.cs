using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerStats stats;
    public Vector2 LastMoveDirection { get; private set; }
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        LastMoveDirection = Vector2.down;
    }

    void Update()
    {
        rb.linearVelocity = moveInput * stats.moveSpeed * stats.speedMultiplier;

        if (moveInput.sqrMagnitude > 0.1f)
        {
            LastMoveDirection = moveInput.normalized;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        moveInput.Normalize();
    }
}

using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInputHandler))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxSpeed = 7f;
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;
    private PlayerInputHandler inputHandler;

    private Vector2 movementInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void OnEnable()
    {
        inputHandler.OnJumpPerformed += HandleJump;
    }

    private void OnDisable()
    {
        inputHandler.OnJumpPerformed -= HandleJump;
    }

    private void Update()
    {
        movementInput = inputHandler.GetMovementInput();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        // Calculate the movement force from input
        Vector3 movementForce = new Vector3(movementInput.x, 0, movementInput.y) * moveSpeed;

        // Apply the force to the rigidbody
        rb.AddForce(movementForce, ForceMode.Force);

        // Get the current velocity but ignore the vertical (Y) component
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        // Check if the horizontal speed has exceeded the max speed
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            // If it has, clamp the velocity to the max speed
            // We preserve the original vertical velocity (rb.velocity.y)
            Vector3 clampedVelocity = horizontalVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(clampedVelocity.x, rb.linearVelocity.y, clampedVelocity.z);
        }
    }

    private void HandleJump()
    {
        if (IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 1.1f);
    }
}
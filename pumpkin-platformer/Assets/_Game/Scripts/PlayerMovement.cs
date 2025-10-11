using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInputHandler))]
public class PlayerMovement : MonoBehaviour
{
    private const float GROUND_CHECK_DISTANCE = 1.1f;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxSpeed = 7f;
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;
    private PlayerInputHandler inputHandler;
    private Camera mainCamera;
    private Vector2 movementInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputHandler = GetComponent<PlayerInputHandler>();
        mainCamera = Camera.main;
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
        
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 desiredDirection = cameraRight * movementInput.x + cameraForward * movementInput.y;

        if (desiredDirection.sqrMagnitude > 1f)
        {
            desiredDirection.Normalize();
        }

        Vector3 movementForce = desiredDirection * moveSpeed;

        rb.AddForce(movementForce, ForceMode.Acceleration);

        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        if (horizontalVelocity.magnitude > maxSpeed)
        {
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
        return Physics.Raycast(transform.position, Vector3.down, GROUND_CHECK_DISTANCE, groundLayer);
    }
}
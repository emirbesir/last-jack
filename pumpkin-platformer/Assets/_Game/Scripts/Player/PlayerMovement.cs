using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInputHandler))]
public class PlayerMovement : MonoBehaviour
{
    private const float GROUND_CHECK_DISTANCE = 1.3f;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveForceMultiplier;
    [SerializeField] private float maxSpeed;
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask groundLayer;

    // References
    private Rigidbody rb;
    private Camera mainCamera;

    // State
    private Vector2 movementInput;
    private bool isCrouching;
    public bool IsCrouching => isCrouching;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        PlayerInputHandler.Instance.OnJumpPerformed += HandleJump;
        PlayerInputHandler.Instance.OnCrouchPerformed += HandleCrouch;
    }

    private void OnDisable()
    {
        PlayerInputHandler.Instance.OnJumpPerformed -= HandleJump;
        PlayerInputHandler.Instance.OnCrouchPerformed -= HandleCrouch;
    }

    private void Update()
    {
        movementInput = PlayerInputHandler.Instance.GetMovementInput();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (isCrouching) return;

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

        Vector3 movementForce = desiredDirection * moveForceMultiplier;

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
        if (isCrouching) return;
        
        if (IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void HandleCrouch()
    {
        isCrouching = !isCrouching;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, GROUND_CHECK_DISTANCE, groundLayer);
    }
}
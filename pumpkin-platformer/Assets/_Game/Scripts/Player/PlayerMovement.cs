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
    private PlayerInputHandler inputHandler;

    // State
    private Vector2 movementInput;
    private bool isCrouching;
    public bool IsCrouching => isCrouching;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        inputHandler = PlayerInputHandler.Instance;
    }

    private void OnEnable()
    {
        var handler = InputHandler;
        if (handler == null) return;

        handler.OnJumpPerformed += HandleJump;
        handler.OnCrouchPerformed += HandleCrouch;
    }

    private void OnDisable()
    {
        var handler = InputHandler;
        if (handler == null) return;

        handler.OnJumpPerformed -= HandleJump;
        handler.OnCrouchPerformed -= HandleCrouch;
    }

    private void Update()
    {
        var handler = InputHandler;
        if (handler == null) return;

        movementInput = handler.GetMovementInput();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (isCrouching) return;

        Vector3 desiredDirection = GetCameraRelativeDirection(movementInput);

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

    private PlayerInputHandler InputHandler
    {
        get
        {
            if (inputHandler == null)
            {
                inputHandler = PlayerInputHandler.Instance;
            }

            return inputHandler;
        }
    }

    private Vector3 GetCameraRelativeDirection(Vector2 input)
    {
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 desiredDirection = cameraRight * input.x + cameraForward * input.y;

        if (desiredDirection.sqrMagnitude > 1f)
        {
            desiredDirection.Normalize();
        }

        return desiredDirection;
    }
}
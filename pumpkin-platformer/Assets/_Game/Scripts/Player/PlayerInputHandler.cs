using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    // References
    private GameInputActions gameInputActions;

    // Events
    public event Action OnJumpPerformed;
    public event Action OnCrouchPerformed;
    public event Action OnInteractPerformed;

    // Singleton
    public static PlayerInputHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        gameInputActions = new GameInputActions();
    }

    private void OnEnable()
    {
        gameInputActions.Enable();
        gameInputActions.Player.Jump.performed += JumpPerformed;
        gameInputActions.Player.Crouch.performed += CrouchPerformed;
        gameInputActions.Player.Interact.performed += InteractPerformed;
    }

    private void OnDisable()
    {
        gameInputActions.Disable();
        gameInputActions.Player.Jump.performed -= JumpPerformed;
        gameInputActions.Player.Crouch.performed -= CrouchPerformed;
        gameInputActions.Player.Interact.performed -= InteractPerformed;
    }

    private void JumpPerformed(InputAction.CallbackContext context)
    {
        OnJumpPerformed?.Invoke();
    }

    private void CrouchPerformed(InputAction.CallbackContext context)
    {
        OnCrouchPerformed?.Invoke();
    }

    private void InteractPerformed(InputAction.CallbackContext context)
    {
        OnInteractPerformed?.Invoke();
    }

    public Vector2 GetMovementInput()
    {
        return gameInputActions.Player.Move.ReadValue<Vector2>();
    }
}

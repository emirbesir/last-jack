using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private GameInputActions gameInputActions;

    public event Action OnJumpPerformed;

    private void Awake()
    {
        gameInputActions = new GameInputActions();
    }

    private void OnEnable()
    {
        gameInputActions.Enable();
        gameInputActions.Player.Jump.performed += JumpPerformed;
    }

    private void OnDisable()
    {
        gameInputActions.Disable();
        gameInputActions.Player.Jump.performed -= JumpPerformed;
    }

    private void JumpPerformed(InputAction.CallbackContext context)
    {
        OnJumpPerformed?.Invoke();
    }

    public Vector2 GetMovementInput()
    {
        return gameInputActions.Player.Move.ReadValue<Vector2>();
    }
}

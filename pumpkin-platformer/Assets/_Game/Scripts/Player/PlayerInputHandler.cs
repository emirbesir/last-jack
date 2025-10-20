using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : SingletonMonoBehaviour<PlayerInputHandler>
{
    // References
    private GameInputActions gameInputActions;

    // Events
    public event Action OnJumpPerformed;
    public event Action OnCrouchPerformed;
    public event Action OnInteractPerformed;
    public event Action OnFlareStarted;
    public event Action OnFlareCanceled;
    public event Action OnSubmitPerformed;
    public event Action OnPausePerformed;

    protected override void Awake()
    {
        base.Awake();

        gameInputActions = new GameInputActions();
    }

    private void OnEnable()
    {
        gameInputActions.Enable();
        gameInputActions.Player.Jump.performed += JumpPerformed;
        gameInputActions.Player.Crouch.performed += CrouchPerformed;
        gameInputActions.Player.Interact.performed += InteractPerformed;
        gameInputActions.Player.Flare.started += FlareStarted;
        gameInputActions.Player.Flare.canceled += FlareCanceled;
        gameInputActions.UI.Submit.performed += SubmitPerformed;
        gameInputActions.UI.Pause.performed += PausePerformed;
    }

    private void OnDisable()
    {
        gameInputActions.Disable();
        gameInputActions.Player.Jump.performed -= JumpPerformed;
        gameInputActions.Player.Crouch.performed -= CrouchPerformed;
        gameInputActions.Player.Interact.performed -= InteractPerformed;
        gameInputActions.Player.Flare.started -= FlareStarted;
        gameInputActions.Player.Flare.canceled -= FlareCanceled;
        gameInputActions.UI.Submit.performed -= SubmitPerformed;
        gameInputActions.UI.Pause.performed -= PausePerformed;
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

    private void FlareStarted(InputAction.CallbackContext context)
    {
        OnFlareStarted?.Invoke();
    }

    private void FlareCanceled(InputAction.CallbackContext context)
    {
        OnFlareCanceled?.Invoke();
    }

    private void SubmitPerformed(InputAction.CallbackContext context)
    {
        OnSubmitPerformed?.Invoke();
    }

    private void PausePerformed(InputAction.CallbackContext context)
    {
        OnPausePerformed?.Invoke();
    }

    public Vector2 GetMovementInput()
    {
        return gameInputActions.Player.Move.ReadValue<Vector2>();
    }
}

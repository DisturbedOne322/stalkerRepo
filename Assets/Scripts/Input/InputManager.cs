using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public event Action OnJumpAction;
    public event Action OnShootAction;
    public event Action OnReloadAction;
    public event Action OnFocusActionStarted;
    public event Action OnFocusActionEnded;
    public event Action OnSprintActionStarted;
    public event Action OnSprintActionEnded;
    public event Action OnHeadlightAction;


    PlayerInputActions playerInputActions;
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();      
    }

    public float GetMovementVector()
    {
         return playerInputActions.Player.Movement.ReadValue<float>();
    }

    public Vector2 GetQTEActions()
    {
        return playerInputActions.Player.QTE.ReadValue<Vector2>();
    }

    private void Start()
    {
        playerInputActions.Player.Focus.started += Focus_started;
        playerInputActions.Player.Focus.canceled += Focus_canceled;
        playerInputActions.Player.Sprint.started += Sprint_started;
        playerInputActions.Player.Sprint.canceled += Sprint_canceled;
        playerInputActions.Player.Headlight.performed += Headlight_performed;
    }

    private void Headlight_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnHeadlightAction?.Invoke();
    }

    private void Sprint_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnSprintActionStarted?.Invoke();
    }

    private void Sprint_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnSprintActionEnded?.Invoke();
    }
    private void Update()
    {
        if(playerInputActions.Player.Jump.triggered)
        {
            OnJumpAction?.Invoke();
            Idle.ReportAction();
        }
        if(playerInputActions.Player.Shoot.triggered)
        {
            OnShootAction?.Invoke();
            Idle.ReportAction();
        }
        if(playerInputActions.Player.Reload.triggered)
        {
            OnReloadAction?.Invoke();
            Idle.ReportAction();
        }
    }

    private void Focus_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnFocusActionStarted?.Invoke();
        Idle.ReportAction();
    }

    private void Focus_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnFocusActionEnded?.Invoke();
        Idle.ReportAction();
    }
}

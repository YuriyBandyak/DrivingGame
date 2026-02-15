using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public event Action OnInteractEvent;

    private InputSystem_Actions.CharacterActions _inputActions;

    public Vector2 Move => _inputActions.Move.ReadValue<Vector2>();
    public Vector2 Look => _inputActions.Look.ReadValue<Vector2>();
    public bool Interact => _inputActions.Interact.triggered;
    public bool Run => _inputActions.Sprint.IsPressed();

    public void Init(InputSystem_Actions.CharacterActions characterActions)
    {
        _inputActions = characterActions;
        _inputActions.Interact.performed += (ctx) =>
        {
            OnInteractEvent?.Invoke();
        };
    }

    public void Enable(bool state = true)
    {
        if (state)
        {
            _inputActions.Enable();
        }
        else
        {
            _inputActions.Disable();
        }
    }

    public void Disable() => Enable(false);
}
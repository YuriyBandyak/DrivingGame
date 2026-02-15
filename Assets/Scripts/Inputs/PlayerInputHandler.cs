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
    }

    // TODO: call init from some manager
    private void Awake()
    {
        var input = new InputSystem_Actions();
        Init(input.Character);
        input.Character.Enable();
        _inputActions.Interact.performed += (ctx) =>
        {
            Debug.Log("Interact performed");
            OnInteractEvent?.Invoke();
        };

        _inputActions.Interact.started += (ctx) =>
        {
            Debug.Log("Interact started");
        };
    }

    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }
}